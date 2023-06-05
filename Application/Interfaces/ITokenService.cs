using Domain.Entities;
using Domain.Common.Models;

namespace Application.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateUserToken(User user, List<RolePermission> rolePermissions);
        Task<AuthToken> GenerateAccessToken(User user);
    }
}
