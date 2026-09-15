using System.Text.RegularExpressions;

namespace NexStructure.Domain.Common.Models;

public static class ValueObjectId
{
    public static T Create<T>(string value)
        where T : struct
    {
        return (T)Activator.CreateInstance(typeof(T), value)!;
    }
    public static bool TryParse<T>(string value, out T id)
        where T : struct
    {
        if (!IsValidPrefix<T>(value))
        {
            id = default;
            return false;
        }
        var result = Create<T>(value);
        id = result;
        return true;
    }
    public static T CreateUnique<T>()
        where T : struct
    {
        var raw = Guid.CreateVersion7().ToString().Replace("-", "");
        var prefix = GetPrefix(typeof(T));
        return Create<T>($"{prefix}_{raw}");
    }

    private static string GetPrefix(Type type)
    {
        var name = type.Name;
        if (name.EndsWith("Id", StringComparison.Ordinal))
            name = name[..^2];

        var parts = Regex.Split(name, "(?=[A-Z])")
            .Where(p => p.Length > 0)
            .ToArray();

        var first = parts.First().ToLowerInvariant();
        var last = parts.Last().ToLowerInvariant();

        var a = first[0];

        var b = last[0];

        char c = first.Length > 1 ? first[1] : 'x';
        if (c == a && first.Length > 2) c = first[2];

        return new string(new[] { a, b, c });
    }

    private static bool IsValidPrefix(string value, Type type)
    {
        var prefix = GetPrefix(type);
        return value.StartsWith($"{prefix}_");
    }

    public static bool IsValidPrefix<T>(string value)
        where T : struct
    {
        var prefix = GetPrefix(typeof(T));
        return value.StartsWith($"{prefix}_");
    }
}