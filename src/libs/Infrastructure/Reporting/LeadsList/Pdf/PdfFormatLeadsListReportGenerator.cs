using Application.Core.Contracts.Reporting;
using Application.Core.Contracts.Repository.Prospecting;
using Domain.Prospecting.Entities;
using HtmlRendererCore.PdfSharp;
using Infrastructure.Configuration;
using PdfSharpCore;
using PdfSharpCore.Pdf;
using Scriban;
using Shared.DataQuerying;
using Shared.Helpers;
using Shared.Results;

namespace Infrastructure.Reporting.LeadsList.Pdf;

public class PdfFormatLeadsListReportGenerator : LeadsListReportGenerator
{
	private readonly ILeadRepository _leadRepository;
	private readonly TimeProvider _timeProvider;

	public PdfFormatLeadsListReportGenerator(
		ILeadRepository leadRepository,
		TimeProvider timeProvider)
	{
		_leadRepository = leadRepository;
		_timeProvider = timeProvider;
	}

	public override async Task<ApplicationResponse<PersistableData>> GenerateAsync(QueryOptions? queryOptions, CancellationToken cancellationToken = default)
	{
		var resourceContent = EmbeddedResourceHelper.GetContent(
								typeof(IInfrastructureAssemblyMarker),
								"Infrastructure.Reporting.LeadsList.Pdf.Template.LeadsListTemplate.html");

		if (string.IsNullOrWhiteSpace(resourceContent))
			return ApplicationResponse<PersistableData>
						.Create(
							default!,
							"O arquivo de modelo para a geração de lista de leads não pôde ser carregado: recurso não encontrado.",
							OperationCodes.Error,
							exception: default,
							inconsistencies: [new Inconsistency(string.Empty, "Erro de configuração.")]);

		var leads = await _leadRepository.GetAllAsync(cancellationToken);
		var renderingResultBytes = GetPdfBytes(Render([.. leads], resourceContent));

		return ApplicationResponse<PersistableData>
					.Create(new(renderingResultBytes,
								renderingResultBytes.Length,
								"application/pdf",
								$"LeadsList-{_timeProvider.GetLocalNow():yyyy-MM-dd_hhmmss}.pdf"));
	}

	private string Render(List<Lead> leads, string template)
		=> Template
			.Parse(template)
			.Render(new { leads.Count, Leads = leads });

	private byte[] GetPdfBytes(string content)
	{
		PdfDocument pdf = new PdfDocument();

		PdfGenerator.AddPdfPages(pdf, content, PageSize.A4);

		using var ms = new MemoryStream();
		pdf.Save(ms);

		return ms.ToArray();
	}
}
