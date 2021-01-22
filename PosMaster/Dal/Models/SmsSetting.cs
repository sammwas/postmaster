namespace PosMaster.Dal
{
	public class SmsSetting : BaseEntity
	{
		public string UserName { get; set; }
		public string ApiKey { get; set; }
		public string SenderId { get; set; }
		public string ShortCode { get; set; }
		public SmsProvider Provider { get; set; }
	}
}
