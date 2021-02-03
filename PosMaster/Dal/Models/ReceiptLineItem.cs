using System;

namespace PosMaster.Dal
{
	public class ReceiptLineItem : BaseEntity
	{
		public Guid ReceiptId { get; set; }
		public Product Product { get; set; }
		public Guid ProductId { get; set; }
		public decimal Quantity { get; set; }
		public decimal SellingPrice { get; set; }
		public decimal UnitPrice { get; set; }
		public decimal Discount { get; set; }
		public decimal TaxRate { get; set; }
		public decimal Amount => (UnitPrice * Quantity) + (UnitPrice * Quantity * TaxRate) - Discount;
        public decimal BuyingPrice { get; set; }
    }
}
