using System.ComponentModel.DataAnnotations;

namespace PosMaster.ViewModels
{
	public class PurchaseOrderViewModel : BaseViewModel
	{
		[Required]
		[Display(Name = "Title")]
		public string Name { get; set; }
		[Required]
		[Display(Name = "Supplier")]
		public string SupplierId { get; set; }
	}
}
