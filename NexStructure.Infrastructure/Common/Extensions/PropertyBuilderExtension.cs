using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexStructure.Domain.Common.Models;

namespace NexStructure.Infrastructure.Common.Extensions
{
    public static class PropertyBuilderExtension
    {
        public static PropertyBuilder<TId> HasValueObjectId<TId>(this PropertyBuilder<TId> builder)
            where TId : struct, IValueObjectId<TId>
        {
            return builder.HasConversion(
                v => v.Value,
                v => ValueObjectId.Create<TId>(v)
                );
        }

        public static PropertyBuilder<TId?> HasNullableValueObjectId<TId>(this PropertyBuilder<TId?> builder)
            where TId : struct, IValueObjectId<TId>
        {
            return builder.HasConversion(
                v => v.HasValue ? v.Value.Value : null,
                v => v == null ? null : ValueObjectId.Create<TId>(v)
            );
        }


        public static PropertyBuilder<TProperty> HasLength<TProperty>(this PropertyBuilder<TProperty> builder, Length length)
            where TProperty : class
        {
            return builder.HasMaxLength(length.Max);
        }
    }
}
