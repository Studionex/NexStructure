using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NexStructure.Domain.Common.Models;

namespace NexStructure.Application.Core.Extensions;

public static class WindowPaginationExtensions
{
  public static async Task<(IReadOnlyList<TDest> Items, bool HasMore, long? MinSequence)> WindowBeforeAsync<TSource, TDest>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, long>> sequenceSelector,
      Expression<Func<TSource, TDest>> projector,
      long beforeSequence,
      int take = 30,
      CancellationToken ct = default)
      where TSource : class
  {
    var items = await source.AsNoTracking()
        .Where(BuildPredicate(sequenceSelector, ExpressionType.LessThan, beforeSequence))
        .OrderByDescending(sequenceSelector)
        .Select(projector)
        .Take(take + 1)
        .ToListAsync(ct);

    var hasMore = items.Count > take;
    if (hasMore) items = items.Take(take).ToList();

    items.Reverse();

    long? minSeq = null;
    if (items.Count > 0)
    {
      var lastBatchMin = await source.AsNoTracking()
          .Where(BuildPredicate(sequenceSelector, ExpressionType.LessThan, beforeSequence))
          .OrderByDescending(sequenceSelector)
          .Select(sequenceSelector)
          .Take(take)
          .MinAsync(ct);

      minSeq = lastBatchMin;
    }

    return (items, hasMore, minSeq);
  }
  public static async Task<(IReadOnlyList<TDest> Items, bool HasMore, long? MaxSequence)> WindowAfterAsync<TSource, TDest>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, long>> sequenceSelector,
      Expression<Func<TSource, TDest>> projector,
      long afterSequence,
      int take = 30,
      CancellationToken ct = default)
      where TSource : class
  {
    var items = await source.AsNoTracking()
        .Where(BuildPredicate(sequenceSelector, ExpressionType.GreaterThan, afterSequence))
        .OrderBy(sequenceSelector)
        .Select(projector)
        .Take(take + 1)
        .ToListAsync(ct);

    var hasMore = items.Count > take;
    if (hasMore) items = items.Take(take).ToList();

    long? maxSeq = null;
    if (items.Count > 0)
    {
      var lastBatchMax = await source.AsNoTracking()
          .Where(BuildPredicate(sequenceSelector, ExpressionType.GreaterThan, afterSequence))
          .OrderBy(sequenceSelector)
          .Select(sequenceSelector)
          .Take(take)
          .MaxAsync(ct);

      maxSeq = lastBatchMax;
    }

    return (items, hasMore, maxSeq);
  }

  public static async Task<WindowResult<TDest>> WindowAroundAsync<TSource, TDest>(
      this IQueryable<TSource> source, // بدون OrderBy ضروري هون، إحنا بنرتبه
      Expression<Func<TSource, long>> sequenceSelector,
      Expression<Func<TSource, TDest>> projector,
      long anchorSequence,
      int before = 15,
      int after = 15,
      bool includeAnchor = true,
      CancellationToken ct = default)
      where TSource : class
  {
    var fromSeq = anchorSequence - before;
    var toSeq = anchorSequence + after;

    var p = sequenceSelector.Parameters[0];
    var seqBody = sequenceSelector.Body;

    var betweenExpr = Expression.Lambda<Func<TSource, bool>>(
        Expression.AndAlso(
            Expression.GreaterThanOrEqual(seqBody, Expression.Constant(fromSeq)),
            Expression.LessThanOrEqual(seqBody, Expression.Constant(toSeq))
        ),
        p);

    IQueryable<TSource> query = source.AsNoTracking().Where(betweenExpr);

    if (!includeAnchor)
    {
      var notAnchorExpr = Expression.Lambda<Func<TSource, bool>>(
          Expression.NotEqual(seqBody, Expression.Constant(anchorSequence)),
          p);

      query = query.Where(notAnchorExpr);
    }

    var items = await query
        .OrderBy(sequenceSelector)
        .Select(projector)
        .ToListAsync(ct);

    if (items.Count == 0)
    {
      var hasMoreBeforeEmpty = await source.AsNoTracking()
          .AnyAsync(BuildPredicate(sequenceSelector, ExpressionType.LessThan, fromSeq), ct);

      var hasMoreAfterEmpty = await source.AsNoTracking()
          .AnyAsync(BuildPredicate(sequenceSelector, ExpressionType.GreaterThan, toSeq), ct);

      return new WindowResult<TDest>(
          Items: items,
          AnchorSequence: anchorSequence,
          MinSequence: null,
          MaxSequence: null,
          HasMoreBefore: hasMoreBeforeEmpty,
          HasMoreAfter: hasMoreAfterEmpty
      );
    }

    var hasMoreBefore = await source.AsNoTracking()
        .AnyAsync(BuildPredicate(sequenceSelector, ExpressionType.LessThan, fromSeq), ct);

    var hasMoreAfter = await source.AsNoTracking()
        .AnyAsync(BuildPredicate(sequenceSelector, ExpressionType.GreaterThan, toSeq), ct);

    var minSeq = await query.Select(sequenceSelector).MinAsync(ct);
    var maxSeq = await query.Select(sequenceSelector).MaxAsync(ct);

    return new WindowResult<TDest>(
        Items: items,
        AnchorSequence: anchorSequence,
        MinSequence: minSeq,
        MaxSequence: maxSeq,
        HasMoreBefore: hasMoreBefore,
        HasMoreAfter: hasMoreAfter
    );
  }

  private static Expression<Func<TSource, bool>> BuildPredicate<TSource>(
      Expression<Func<TSource, long>> selector,
      ExpressionType comparison,
      long value)
  {
    var p = selector.Parameters[0];
    var body = selector.Body;

    Expression cmp = comparison switch
    {
      ExpressionType.LessThan => Expression.LessThan(body, Expression.Constant(value)),
      ExpressionType.GreaterThan => Expression.GreaterThan(body, Expression.Constant(value)),
      _ => throw new NotSupportedException($"Comparison {comparison} not supported.")
    };

    return Expression.Lambda<Func<TSource, bool>>(cmp, p);
  }
}
