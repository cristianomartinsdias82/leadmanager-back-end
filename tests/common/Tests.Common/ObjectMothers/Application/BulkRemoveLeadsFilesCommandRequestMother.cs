using Application.Prospecting.Leads.Commands.BulkRemoveLeadsFiles;
using Tests.Common.ObjectMothers.Domain;

namespace Tests.Common.ObjectMothers.Application;

public static class BulkRemoveLeadsFilesCommandRequestMother
{
	public static BulkRemoveLeadsFilesCommandRequest WithIds(IEnumerable<RemoveLeadsFileDto> ids)
		=> new() { Ids = ids };

	public static BulkRemoveLeadsFilesCommandRequest Default()
		=> new() { Ids = LeadsFileMother.LeadsFiles().Select(it => new RemoveLeadsFileDto(it.Id, it.FileId)) };
}