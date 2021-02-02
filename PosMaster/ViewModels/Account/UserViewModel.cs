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
		}
		[HiddenInput]
		public string UserId { get; set; }
		[DataType(DataType.EmailAddress)]
		[Required]
		public string EmailAddress { get; set; }
		[Required]
		public string PhoneNumber { get; set; }
		public bool EmailConfirmed { get; set; }
		[Required]
		public string Role { get; set; }
		public DateTime DateCreated { get; set; }
		public string ImagePath { get; set; }
		[Required]
		public string IdNumber { get; set; }
		[Required]
		public string Gender { get; set; }
		[Required]
		public string Title { get; set; }
		[Required]
		public string FirstName { get; set; }
		public string MiddleName { get; set; }
		public string LastName { get; set; }
		public string MaritalStatus { get; set; }
		public string FullName => $"{FirstName} {MiddleName} {LastName}";
		public bool IsNewImage { get; set; }
		[Display(Name = "Date of Birth")]
		public string DoB { get; set; }
	}
}
