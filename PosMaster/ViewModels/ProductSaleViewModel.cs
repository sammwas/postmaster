using System.ComponentModel.DataAnnotations;

namespace PosMaster.ViewModels
{
	public class ProductSaleViewModel : BaseViewModel
	{
		[Display(Name = "Unit Price")]
		public decimal UnitPrice { get; set; }
		public decimal Discount { get; set; }
		[Required]
		[Display(Name = "Customer")]
		public string CustomerId { get; set; }
		[Required]
		[Display(Name = "Product")]
		public string ProductId { get; set; }
		public decimal Quantity { get; set; }
		[Required]
		[Display(Name = "Payment Mode")]
		public string PaymentMode { get; set; }
		[Display(Name = "External Ref")]
		public string ExternalRef { get; set; }
		public bool IsCredit { get; set; }
		public bool IsWalkIn { get; set; }
	}
}
