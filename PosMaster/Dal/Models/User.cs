using Microsoft.AspNetCore.Identity;
using System;

namespace PosMaster.Dal
{
	public class User : IdentityUser
	{
		public User()
		{
			DateCreated = DateTime.Now;
		}
		public string Role { get; set; }
		public DateTime DateCreated { get; set; }
		public DateTime? DateLastModified { get; set; }
		public Guid ClientId { get; set; }
		public Client Client { get; set; }
		public Guid InstanceId { get; set; }
		public string Personnel { get; set; }
		public string LastModifiedBy { get; set; }
		public string Notes { get; set; }
		public string ImagePath { get; set; }
		public string IdNumber { get; set; }
		public string Gender { get; set; }
		public string Title { get; set; }
		public string FirstName { get; set; }
		public string MiddleName { get; set; }
		public string LastName { get; set; }
		public string MaritalStatus { get; set; }
		public EntityStatus Status { get; set; }
		public DateTime? PasswordChangeDate { get; set; }
		public DateTime DateOfBirth { get; set; }
		public string FullName => $"{Title} {FirstName} {MiddleName} {LastName}";
	}
}
