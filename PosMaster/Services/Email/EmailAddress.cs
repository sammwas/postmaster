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
			Name = user.FullName;
			Address = user.Email;
			ClientId = user.ClientId;
			Id = user.Id;
			InstanceId = user.InstanceId;
		}
		public string Id { get; set; }
		public string Name { get; set; }
		public string Address { get; set; }
		public Guid ClientId { get; set; }
		public Guid InstanceId { get; set; }
	}
}
