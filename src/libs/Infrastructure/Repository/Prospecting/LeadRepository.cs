using Application.Core.Contracts.Persistence;
using Application.Core.Contracts.Repository.Prospecting;
using Domain.Prospecting.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.DataPagination;
using System.Linq.Expressions;

namespace Infrastructure.Repository.Prospecting;

internal sealed class LeadRepository : RepositoryBase<Lead>, ILeadRepository
{
    private readonly ILeadManagerDbContext _leadDbContext;

    public LeadRepository(ILeadManagerDbContext leadDbContext)
    {
        _leadDbContext = leadDbContext;
    }

    public override async Task AddAsync(Lead lead, CancellationToken cancellationToken = default)
        => await _leadDbContext.Leads.AddAsync(lead, cancellationToken);

    public override async Task AddRangeAsync(IEnumerable<Lead> leads, CancellationToken cancellationToken = default)
        => await _leadDbContext.Leads.AddRangeAsync(leads, cancellationToken);

    public override async Task<PagedList<Lead>> GetAsync(
        PaginationOptions paginationOptions,
        string? search = default,
        CancellationToken cancellationToken = default)
        => PagedList<Lead>.Paginate(
            await GenerateSearchQueryExpression(_leadDbContext.Leads.AsNoTracking(), search)
                    .ToListAsync(cancellationToken),
                        PaginationOptions
                            .SinglePage()
                            .WithSortColumn(paginationOptions.SortColumn)
                            .WithSortDirection(paginationOptions.SortDirection));

    public override async Task<IEnumerable<Lead>> GetAllAsync(CancellationToken cancellationToken = default)
       => await _leadDbContext
                    .Leads
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

    public override async Task<Lead?> GetByIdAsync(
        Guid id,
        bool? bypassCacheLayer = false,
        CancellationToken cancellationToken = default)
        => await _leadDbContext.Leads.FindAsync([id], cancellationToken: cancellationToken);

    public override async Task RemoveAsync(Lead lead, CancellationToken cancellationToken = default)
    {
        if (await _leadDbContext.Leads.AnyAsync(ld => ld.Id == lead.Id, cancellationToken))
            _leadDbContext.Leads.Remove(lead);
    }

    public override async Task UpdateAsync(Lead lead, byte[] rowVersion, CancellationToken cancellationToken = default)
    {
        if (await _leadDbContext.Leads.AnyAsync(ld => ld.Id == lead.Id, cancellationToken))
        {
            _leadDbContext.SetStateToModified(lead);
            _leadDbContext.SetConcurrencyToken(lead, nameof(Lead.RowVersion), rowVersion);
        }
    }

    public override async Task<bool> ExistsAsync(
        Expression<Func<Lead, bool>> matchCriteria,
        CancellationToken cancellationToken = default)
        => await _leadDbContext
                    .Leads
                    .AsNoTracking()
                    .AnyAsync(matchCriteria, cancellationToken);

    public static IQueryable<Lead> GenerateSearchQueryExpression(
        IQueryable<Lead> queryable,
        string? search = default)
        => queryable.Where(it => string.IsNullOrWhiteSpace(search) ||
                                 it.Cnpj.Contains(search) ||
                                 it.RazaoSocial.ToUpper().Contains(search, StringComparison.InvariantCultureIgnoreCase));
}
