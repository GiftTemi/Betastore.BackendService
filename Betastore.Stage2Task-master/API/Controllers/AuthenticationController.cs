using Application.Authentication.Login;
using Application.Common.Exceptions;
using Application.Users.Commands;
using Domain.Common.Models;
using Infrastructure.Utility;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class AuthenticationController : ApiController
    {
        [HttpPost("login")]
        public async Task<ActionResult<AuthResult>> Login(LoginCommand command)
        {
            try
            {
                if (command == null)
                    return new AuthResult { IsSuccess = false, Message = "Bad Request" };
                return await Mediator.Send(command);
            }
            catch (ValidationException ex)
            {
                return new AuthResult { IsSuccess = false, Message = ex?.Message ?? ex?.InnerException?.Message + "" + "Error:" + ex.GetErrors() };
            }
            catch (System.Exception ex)
            {
                return new AuthResult { IsSuccess = false, Message = " Authentication login was not successful: " + ex?.Message + ex?.InnerException?.Message };
            }
        }
        [HttpPost("signup")]
        public async Task<ActionResult<Result>> Create(CreateUserCommand command)
        {
            try
            {
                if (command == null)
                {
                    return Result.Failure($"Invalid request!");
                }
                return await Mediator.Send(command);
            }
            catch (ValidationException ex)
            {
                return Result.Failure($"{ex?.Message ?? ex?.InnerException?.Message}. Error: {ex.GetErrors()}");
            }
            catch (Exception ex)
            {
                return Result.Failure($"User creation was not successful: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        //[HttpPost("logout")]
        //public async Task<ActionResult<Result>> LogOut(LogoutCommand command)
        //{

        //    try
        //    {
        //        if (command == null)
        //            return Result.Failure("Bad Request");
        //        return await Mediator.Send(command);
        //    }
        //    catch (ValidationException ex)
        //    {
        //        return Result.Failure($"{ex?.Message ?? ex?.InnerException?.Message}. Error: {ex.GetErrors()}");
        //    }
        //    catch (System.Exception ex)
        //    {
        //        return Result.Failure($"Authentication logout was not successful {ex?.Message ?? ex?.InnerException?.Message}");
        //    }
        //}


        //[HttpPost("changepassword")]
        //public async Task<ActionResult<Result>> ChangePassword(ChangePasswordCommand command)
        //{
        //    try
        //    {
        //        if (command == null)
        //            return Result.Failure("Bad Request");
        //        return await Mediator.Send(command);
        //    }
        //    catch (ValidationException ex)
        //    {
        //        return Result.Failure($"{ex?.Message ?? ex?.InnerException?.Message}. Error: {ex.GetErrors()}");
        //    }
        //    catch (System.Exception ex)
        //    {
        //        return Result.Failure($"Change Password update was not successful {ex?.Message ?? ex?.InnerException?.Message}");
        //    }
        //}
        //[HttpPost("forgotpassword")]
        //public async Task<ActionResult<Result>> ForgotPassword(ForgotPasswordCommand command)
        //{
        //    try
        //    {
        //        if (command == null)
        //            return Result.Failure("Bad Request");
        //        return await Mediator.Send(command);
        //    }
        //    catch (ValidationException ex)
        //    {
        //        return Result.Failure($"{ex?.Message ?? ex?.InnerException?.Message}. Error: {ex.GetErrors()}");
        //    }
        //    catch (System.Exception ex)
        //    {
        //        return Result.Failure($"Forgot Password was not successful {ex?.Message ?? ex?.InnerException?.Message}");
        //    }

        //}
        //[HttpPost("resetpassword")]
        //public async Task<ActionResult<Result>> ResetPassword(ResetPasswordCommand command)
        //{
        //    try
        //    {
        //        if (command == null)
        //            return Result.Failure("Bad Request");
        //        return await Mediator.Send(command);
        //    }
        //    catch (ValidationException ex)
        //    {
        //        return Result.Failure($"{ex?.Message ?? ex?.InnerException?.Message}. Error: {ex.GetErrors()}");
        //    }
        //    catch (Exception ex)
        //    {
        //        return Result.Failure($"Reset Password Completion was not successful {ex?.Message ?? ex?.InnerException?.Message}");
        //    }
        //}

        //[HttpPost("verifyemail/{email}")]
        //public async Task<ActionResult<Result>> VerifyEmail(string email, VerifyUserEmailCommand command)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(email))
        //            return Result.Failure("Please provide a valid email");
        //        command.Email = email;
        //        return await Mediator.Send(command);
        //    }
        //    catch (ValidationException ex)
        //    {
        //        return Result.Failure($"{ex?.Message ?? ex?.InnerException?.Message}. Error: {ex.GetErrors()}");
        //    }
        //    catch (Exception ex)
        //    {
        //        return Result.Failure($"Email verification was not successful {ex?.Message ?? ex?.InnerException?.Message}");
        //    }
        //}
    }

}
