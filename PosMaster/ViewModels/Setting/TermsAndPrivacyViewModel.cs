using System.ComponentModel.DataAnnotations;

namespace PosMaster.ViewModels
{
	public class TermsAndPrivacyViewModel
	{
		[Required]
		public string Terms { get; set; }
		[Required]
		public string Privacy { get; set; }
		[Required]
		[Display(Name = "Home")]
		public string Notes { get; set; }
	}
}
