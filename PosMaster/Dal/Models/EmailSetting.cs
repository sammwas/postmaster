using MailKit.Security;

namespace PosMaster.Dal
{
	public class EmailSetting : BaseEntity
	{
		public string SmtpServer { get; set; }
		public int SmtpPort { get; set; }
		public string SmtpUsername { get; set; }
		public string SmtpPassword { get; set; }
		public string SenderFromEmail { get; set; }
		public string SenderFromName { get; set; }
		public SecureSocketOptions SocketOptions { get; set; }
	}
}
