using Microsoft.EntityFrameworkCore;
using NexStructure.Domain.Common.Models;

namespace NexStructure.Application.Core.Extensions;

public static class PaginatedListExtension
{

    public static Task<PaginatedList<TDestination>> PaginatedListAsync<TDestination>(this IQueryable<TDestination> queryable, int pageNumber, int pageSize, CancellationToken ct = default) where TDestination : class
            => PaginatedList<TDestination>.CreateAsync(queryable.AsNoTracking(), pageNumber, pageSize, ct);
}
