using System;

namespace PosMaster.Dal
{
	public class UserLoginLog : BaseEntity
	{
		public string UserName { get; set; }
		public string Agent { get; set; }
		public string RefererUrl { get; set; }
		public bool IsHttps { get; set; }
		public bool Success { get; set; }
		public string Message { get; set; }
		public string IpAddress { get; set; }
		public DateTime? LogoutTime { get; set; }
	}
}
