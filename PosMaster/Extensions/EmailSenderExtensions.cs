using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PosMaster.Extensions
{
    public static class EmailSenderExtensions
    {
        public static Task SendEmailConfirmationAsync(this IEmailSender emailSender, EmailAddress address, string link)
        {
            return emailSender.SendEmailAsync(address, "Confirm your Email",
                $"Dear {address.Name}, <br/>" +
                $"<br/>Please confirm your account by clicking this link: <a href='{HtmlEncoder.Default.Encode(link)}'>link</a>");
        }

        public static Task SendSupervisorAssignedAsync(this IEmailSender emailSender, EmailAddress address, string link)
        {
            return emailSender.SendEmailAsync(address, "Institution Supervisor Assigned",
                $"Dear {address.Name}, <br/><br/>You have been assigned an attachment to supervise. " +
                $"<br/>View the attachment by clicking this link: <a href='{HtmlEncoder.Default.Encode(link)}'>link</a>"
                , true);
        }

        public static Task SendStudentApprovedAsync(this IEmailSender emailSender, EmailAddress address, string link)
        {
            return emailSender.SendEmailAsync(address, "Attachment Approved",
                $"Dear {address.Name}, <br/><br/>Your attachment has been approved. Update the Industrial supervisor. " +
                $"<br/>View the attachment by clicking this link: <a href='{HtmlEncoder.Default.Encode(link)}'>link</a>",
                true);
        }

        public static Task SendStudentDeclinedAsync(this IEmailSender emailSender, EmailAddress address, string link)
        {
            return emailSender.SendEmailAsync(address, "Attachment Declined",
                $"Dear {address.Name}, <br/><br/>Your attachment has been Declined. " +
                $"<br/> Kindly visit the portal for more details.",
                true);
        }
    }
}
