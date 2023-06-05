using Domain.Common;
using Domain.Enums;

namespace Domain.Entities
{
    public class PasswordResetAttempt : AuditableEntity
    {
        public PasswordResetStatus PasswordResetStatus { get; set; }
        public string PasswordResetStatusDesc => PasswordResetStatus.ToString();
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
