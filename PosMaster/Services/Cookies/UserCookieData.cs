using PosMaster.Dal;
using System;

namespace PosMaster.Services
{
	public class UserCookieData
	{
		public UserCookieData()
		{

		}

		public UserCookieData(User user)
		{
			UserId = user.Id;
			FirstName = user.FirstName;
			FullName = user.FullName;
			ClientId = user.ClientId;
			InstanceId = user.InstanceId;
			Role = user.Role;
		}
		public string UserId { get; set; }
		public string FirstName { get; set; }
		public string FullName { get; set; }
		public string Role { get; set; }
		public Guid ClientId { get; set; }
		public string ClientCode { get; set; }
		public string ClientName { get; set; }
		public string ClientLogoPath { get; set; }
		public Guid InstanceId { get; set; }
		public string InstanceCode { get; set; }
		public string InstanceName { get; set; }
	}
}
