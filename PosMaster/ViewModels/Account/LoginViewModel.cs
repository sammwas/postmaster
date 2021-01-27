using System.ComponentModel.DataAnnotations;

namespace PosMaster.ViewModels
{
	public class LoginViewModel
	{
		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; }
		[Display(Name = "Remember me?")]
		public bool RememberMe { get; set; }
		[Required]
		[Display(Name = "Email Address")]
		[DataType(DataType.EmailAddress)]
		public string EmailAddress { get; set; }
		public bool IsHttps { get; set; }
	}
}
