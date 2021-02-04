using System.ComponentModel.DataAnnotations;

namespace PosMaster.ViewModels
{
	public class PoViewModel : BaseViewModel
	{
		[Required]
		[Display(Name = "Title")]
		public string Name { get; set; }
		[Required]
		[Display(Name = "Supplier")]
		public string SupplierId { get; set; }
		public string ProductsItemsListStr { get; set; }
	}
}
