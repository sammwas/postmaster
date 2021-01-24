using System.ComponentModel.DataAnnotations;

namespace PosMaster.ViewModels
{
	public class ForgotPasswordViewModel
	{
		[Required]
		[Display(Name = "Email Address")]
		[DataType(DataType.EmailAddress)]
		public string EmailAddress { get; set; }
	}
}
