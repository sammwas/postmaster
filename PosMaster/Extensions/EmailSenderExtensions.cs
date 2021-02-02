using PosMaster.Dal.Interfaces;
using PosMaster.Services;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace PosMaster.Extensions
{
	public static class EmailSenderExtensions
	{
		public static Task<ReturnData<string>> SendEmailConfirmationAsync(this IEmailService emailSender, EmailAddress address, string link)
		{
			return emailSender.SendAsync(address, "Confirm your Email",
				$"Dear {address.Name}, <br/>" +
				$"<br/>Please confirm your account by clicking this link: <a href='{HtmlEncoder.Default.Encode(link)}'>link</a>");
		}

		public static Task SendSupervisorAssignedAsync(this IEmailService emailSender, EmailAddress address, string link)
		{
			return emailSender.SendAsync(address, "Institution Supervisor Assigned",
				$"Dear {address.Name}, <br/><br/>You have been assigned an attachment to supervise. " +
				$"<br/>View the attachment by clicking this link: <a href='{HtmlEncoder.Default.Encode(link)}'>link</a>");
		}

		public static Task SendStudentApprovedAsync(this IEmailService emailSender, EmailAddress address, string link)
		{
			return emailSender.SendAsync(address, "Attachment Approved",
				$"Dear {address.Name}, <br/><br/>Your attachment has been approved. Update the Industrial supervisor. " +
				$"<br/>View the attachment by clicking this link: <a href='{HtmlEncoder.Default.Encode(link)}'>link</a>");
		}


	}
}
