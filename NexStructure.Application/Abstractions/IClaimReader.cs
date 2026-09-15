using System.Diagnostics.CodeAnalysis;
using NexStructure.Domain.Common.Models;

namespace NexStructure.Application.Abstractions;

public interface IClaimReader
{
    public string? GetUserId();
    public bool TryGetId<T>([NotNullWhen(true)] out T result)where T : IValueObjectId<T>;
    public List<string> GetUserRoles();
    public bool HasRoles(params Roles[] requiredRoles);
    public string? GetClaimValue(string claimType);
    


}