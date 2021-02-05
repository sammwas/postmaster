using System.ComponentModel.DataAnnotations;

namespace PosMaster.ViewModels
{
	public class TermsAndPrivacyViewModel
	{
		[Required]
		public string Terms { get; set; }
		[Required]
		public string Privacy { get; set; }
	}
}
