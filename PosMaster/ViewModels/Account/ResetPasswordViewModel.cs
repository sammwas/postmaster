using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace PosMaster.ViewModels
{
	public class ResetPasswordViewModel
	{	 
		[HiddenInput]
		public string Id { get; set; }
		[Required]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "New Password")]
		public string Password { get; set; }
		[DataType(DataType.Password)]
		[Display(Name = "Confirm Password")]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }
		[HiddenInput]
		public string Code { get; set; }
	}
}
