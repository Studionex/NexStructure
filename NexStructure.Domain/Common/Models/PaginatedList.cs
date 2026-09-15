using Microsoft.EntityFrameworkCore;

namespace NexStructure.Domain.Common.Models;

public class PaginatedList<T>
{
    public int PageNumber { get; }
    public int TotalPages { get; }
    public int PageSize { get; }
    public int TotalCount { get; }

    public PaginatedList(IReadOnlyCollection<T> items, int count, int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        TotalCount = count;
        PageSize = pageSize;
        TotalPages = TotalCount > 0 ? (int)Math.Ceiling(count / (double)pageSize) : 0;
        Items = items;
    }

    public bool HasPreviousPage => PageNumber > 1;

    public bool HasNextPage => PageNumber < TotalPages;
    public IReadOnlyCollection<T> Items { get; }

    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var count = await source.CountAsync(cancellationToken: cancellationToken);
        var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken: cancellationToken);

        return new PaginatedList<T>(items, count, pageNumber, pageSize);
    }



    public static PaginatedList<T> Empty(int pageSize) =>
        new(Array.Empty<T>(), 0, 1, pageSize);
    public static PaginatedList<T> From(IReadOnlyCollection<T> items, int totalCount, int pageNumber, int pageSize) =>
        new(items, totalCount, pageNumber, pageSize);

}

public sealed record WindowResult<T>(
    IReadOnlyList<T> Items,
    long AnchorSequence,
    long? MinSequence,
    long? MaxSequence,
    bool HasMoreBefore,
    bool HasMoreAfter)
{
    public static WindowResult<T> Empty(long anchor) =>
        new(Array.Empty<T>(), anchor, null, null, false, false);
}
