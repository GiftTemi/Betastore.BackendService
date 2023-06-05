using Domain.Common.Models;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IIdentityService
    {
        Task<(Result Result, string UserId)> CreateUserAsync(User user);
        Task<(Result Result, User user)> GetUserByEmail(string email);
        Task<Result> ChangeUserStatusAsync(User user);
        Task<Result> ChangeUserStatusAsync(User user, string message);
        Task<bool> CheckPasswordAsync(User user, string password);
        Task<(Result result, List<User> users)> GetAll(int skip, int take);
        Task<(Result result, List<User> users)> GetAllCustomers(int skip, int take);
        Task<(Result result, List<User> users)> GetAll(int skip, int take, string searchvalue, List<string> filter);
        Task<(Result Result, User user)> GetDefaultAdmin();
        Task<(Result Result, User user)> GetUserByUsernameOrEmail(string email,string username);
        Task<(Result Result, User user)> GetUserById(string userId);
        Task<(Result Result, User user)> GetUserByUsername(string userName);
        Task<(Result Result, string userName)> GetUserNameAsync(string userId);
        Task<(Result result, List<User> users)> GetUsersByDepartment(string role);
        Task<bool> RoleInUse(int roleId);
        Task<Result> UpdateUserAsync(User user);
    }

}
