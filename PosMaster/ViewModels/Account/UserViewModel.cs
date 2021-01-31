using PosMaster.Dal;
using System;

namespace PosMaster.ViewModels
{
	public class UserViewModel : BaseViewModel
	{
		public UserViewModel()
		{

		}
		public UserViewModel(User user)
		{
			UserId = user.Id;
			ClientId = user.ClientId;
			InstanceId = user.InstanceId;
			Notes = user.Notes;
			Status = user.Status;
			EmailAddress = user.Email;
			PhoneNumber = user.PhoneNumber;
			EmailConfirmed = user.EmailConfirmed;
			Role = user.Role;
			DateCreated = user.DateCreated;
			ImagePath = user.ImagePath;
			IdNumber = user.IdNumber;
			Gender = user.Gender;
			Title = user.Title;
			FirstName = user.FirstName;
			MiddleName = user.MiddleName;
			LastName = user.LastName;
			MaritalStatus = user.MaritalStatus;
			IsEditMode = true;
		}
		public string UserId { get; set; }
		public string EmailAddress { get; set; }
		public string PhoneNumber { get; set; }
		public bool EmailConfirmed { get; set; }
		public string Role { get; set; }
		public DateTime DateCreated { get; set; }
		public string ImagePath { get; set; }
		public string IdNumber { get; set; }
		public string Gender { get; set; }
		public string Title { get; set; }
		public string FirstName { get; set; }
		public string MiddleName { get; set; }
		public string LastName { get; set; }
		public string MaritalStatus { get; set; }
		public string FullName => $"{FirstName} {MiddleName} {LastName}";
		public bool IsNewImage { get; set; }
	}
}
