using System.ComponentModel.DataAnnotations;

namespace PosMaster.ViewModels
{
	public class OrderViewModel : BaseViewModel
	{
		[Required]
		[Display(Name = "Title")]
		public string Name { get; set; }
		[Required]
		[Display(Name = "Customer")]
		public string CustomerId { get; set; }
		public string LineItemListStr { get; set; }
	}
}
