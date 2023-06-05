using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Common.Models
{
    public class EmailVm
    {
        public string Application { get; set; }
        public string Subject { get; set; }
        public string RecipientName { get; set; }
        public string Text { get; set; }
        public string ButtonLink { get; set; }
        public string Body { get; set; }
        public string Body1 { get; set; }
        public string Body2 { get; set; }
        public string Body3 { get; set; }
        public string BCC { get; set; }
        public string CC { get; set; }
        public string DisplayButton { get; set; }
        public string ImageSource { get; set; }
        public string ButtonText { get; set; }
        public string RecipientEmail { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NewPassword { get; set; }
        public string OldPassword { get; set; }
        public string OrganizationCode { get; set; }
    }
}
