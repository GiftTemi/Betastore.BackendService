using Domain.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IEmailService
    {
        Task ChangePasswordEmail(EmailVm email);
        Task ResetPasswordEmailAsync(EmailVm email);
        Task SendForgotPasswordEmailAsync(EmailVm email);
    }
}
