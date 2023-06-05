using Application.Interfaces;
using Domain.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Utility
{
    public class EmailService : IEmailService
    {
        public Task ChangePasswordEmail(EmailVm email)
        {
            throw new NotImplementedException();
        }

        public Task ResetPasswordEmailAsync(EmailVm email)
        {
            throw new NotImplementedException();
        }

        public Task SendForgotPasswordEmailAsync(EmailVm email)
        {
            throw new NotImplementedException();
        }
    }
}
