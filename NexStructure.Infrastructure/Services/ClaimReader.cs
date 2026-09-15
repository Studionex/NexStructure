using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using NexStructure.Application.Abstractions;
using NexStructure.Domain.Common.Models;

namespace NexStructure.Infrastructure.Services;

public class ClaimReader(IHttpContextAccessor httpContextAccessor):IClaimReader
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public string? GetUserId()
    {
        return _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    public bool TryGetId<T>(out T result) where T : IValueObjectId<T>
    {
        var id = GetUserId();

        if (id is null)
        {
            result = default!;
            return false;
        }

        if (!T.TryParse(id, out var parsedId))
        {
            result = default!;
            return false;
        }
        result = parsedId;
        return true;
    }

    public List<string> GetUserRoles()
    {
        var roles = new List<string>();
        var roleClaims = _httpContextAccessor.HttpContext?.User.FindAll(ClaimTypes.Role);
        if (roleClaims != null)
        {
            foreach (var roleClaim in roleClaims)
            {
                roles.Add(roleClaim.Value);
            }
        }
        return roles;
    }

    public bool HasRoles(params Roles[] requiredRoles)
    {
        /*var permissions = new List<Roles>();
        var permissionClaims = _httpContextAccessor.HttpContext?.User.FindAll(ClaimTypes.Role)
            .Select(c => c.Value).ToList();
        if (permissionClaims != null)
        {
            permissions = PermissionHelper.TryGetPermissionFromStringList(permissionClaims);
        }
        return permissions;*/
        throw new NotImplementedException();
    }

    public string? GetClaimValue(string claimType)
    {
        return _httpContextAccessor.HttpContext?.User.FindFirst(claimType)?.Value;
    }
}