using Domain.Common.Models;

namespace Application.Interfaces
{
    public interface IAuthenticateService
    {
        Task<AuthResult> Login(string username, string password);
        Task<Result> ResetPassword(string userId, string password);
        Task<Result> LogOut(string username);

        Task<Result> ChangePassword(string userName, string oldPassword, string newPassword);
        Task<Result> ForgotPassword(string email);
    }
}
