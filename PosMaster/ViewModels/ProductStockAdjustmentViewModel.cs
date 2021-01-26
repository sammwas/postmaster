using System.ComponentModel.DataAnnotations;

namespace PosMaster.ViewModels
{
	public class ProductStockAdjustmentViewModel : BaseViewModel
	{
		[Required]
		[Display(Name = "Product")]
		public string ProductId { get; set; }
		[Display(Name = "Final Quantity")]
		public decimal QuantityTo { get; set; }
	}
}
