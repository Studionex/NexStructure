namespace NexStructure.Domain.Common.Models;

public interface IValueObjectId<T>
{
    public string Value { get; }
    public static abstract T Create();
    public static abstract T From(string value);
    public static abstract bool TryParse(string value, out T id);
}