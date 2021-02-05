using Microsoft.AspNetCore.Mvc;
using PosMaster.Dal;
using System;
using System.ComponentModel.DataAnnotations;

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
			UserInstanceId = user.InstanceId.ToString();
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
			DoB = user.DateOfBirth.ToString("dd-MMM-yyyy");
			FullName = user.FullName;
		}
		[HiddenInput]
		public string UserId { get; set; }
		[Required]
		[Display(Name = "Instance")]
		public string UserInstanceId { get; set; }
		[DataType(DataType.EmailAddress)]
		[Required]
		[Display(Name = "Email Address")]
		public string EmailAddress { get; set; }
		[Required]
		[Display(Name = "Phone Number")]
		public string PhoneNumber { get; set; }
		[Display(Name = "Email Confirmed ")]
		public bool EmailConfirmed { get; set; }
		[Required]
		public string Role { get; set; }
		public DateTime DateCreated { get; set; }
		public string ImagePath { get; set; }
		[Required]
		[Display(Name = "Id Number")]
		public string IdNumber { get; set; }
		[Required]
		public string Gender { get; set; }
		[Required]
		public string Title { get; set; }
		[Required]
		[Display(Name = "First Name")]
		public string FirstName { get; set; }
		[Display(Name = "Middle Name")]
		public string MiddleName { get; set; }
		[Display(Name = "Last Name")]
		public string LastName { get; set; }
		public string MaritalStatus { get; set; }
		public string FullName { get; set; }
		public bool IsNewImage { get; set; }
		[Display(Name = "Date of Birth")]
		public string DoB { get; set; }
		public bool HasPassword { get; set; }
	}
}
