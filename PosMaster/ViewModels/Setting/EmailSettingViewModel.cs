using MailKit.Security;
using PosMaster.Dal;
using System.ComponentModel.DataAnnotations;

namespace PosMaster.ViewModels
{
	public class EmailSettingViewModel : BaseViewModel
	{
		public EmailSettingViewModel()
		{

		}

		public EmailSettingViewModel(EmailSetting setting)
		{
			Id = setting.Id;
			ClientId = setting.ClientId;
			InstanceId = setting.InstanceId;
			SmtpServer = setting.SmtpServer;
			SmtpPort = setting.SmtpPort;
			SmtpUsername = setting.SmtpUsername;
			SmtpPassword = setting.SmtpPassword;
			SenderFromEmail = setting.SenderFromEmail;
			SenderFromName = setting.SenderFromName;
			SocketOptions = setting.SocketOptions;
			IsEditMode = true;
		}

		[Display(Name = "SMTP Server")]
		public string SmtpServer { get; set; }
		[Display(Name = "SMTP Port")]
		public int SmtpPort { get; set; }
		[Display(Name = "SMTP Username")]
		public string SmtpUsername { get; set; }
		[Display(Name = "SMTP Password")]
		public string SmtpPassword { get; set; }
		[Display(Name = "Sender Email")]
		public string SenderFromEmail { get; set; }
		[Display(Name = "Sender Name")]
		public string SenderFromName { get; set; }
		[Display(Name = "Secure Options")]
		public SecureSocketOptions SocketOptions { get; set; }
	}
}
