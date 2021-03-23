using PosMaster.Dal;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PosMaster.ViewModels
{
	public class ProductStockAdjustmentViewModel : BaseViewModel
	{
        public ProductStockAdjustmentViewModel()
        {
			Products = new List<Product>();
        }
		[Required]
		[Display(Name = "Product")]
		public string ProductId { get; set; }
		[Display(Name = "Final Quantity")]
		public decimal QuantityTo { get; set; }
		[Display(Name = "Current Quantity")]
		public decimal CurrentQuantity { get; set; }
        public List<Product> Products { get; set; }
    }
}
