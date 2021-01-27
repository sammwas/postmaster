using PosMaster.Dal;
using System;

namespace PosMaster.Services
{
	public class EmailAddress
	{
		public EmailAddress()
		{

		}

		public EmailAddress(User user)
		{
			Id = user.Id;
			Name = user.FullName;
			Address = user.Email;
			ClientId = user.ClientId;
			InstanceId = user.InstanceId;
		}

		public EmailAddress(UserCookieData data)
		{
			Id = data.UserId;
			Name = data.FullName;
			Address = data.EmailAddress;
			ClientId = data.ClientId;
			InstanceId = data.InstanceId;
		}

		public string Id { get; set; }
		public string Name { get; set; }
		public string Address { get; set; }
		public Guid ClientId { get; set; }
		public Guid InstanceId { get; set; }
	}
}
