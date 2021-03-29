using System;

namespace PosMaster.Dal
{
	public class OrderLineItem : BaseEntity
	{
		public Guid OrderId { get; set; }
		public Product Product { get; set; }
		public Guid ProductId { get; set; }
		public decimal Quantity { get; set; }
		public decimal SellingPrice { get; set; }
		public decimal UnitPrice { get; set; }
		public decimal Discount { get; set; }
		public decimal TaxRate { get; set; }
		public decimal Amount => SellingPrice * Quantity;
		public decimal ActualProfit => (UnitPrice * Quantity) - (BuyingPrice * Quantity);
		public decimal ExpectedProfit => (SellingPrice * Quantity) - (BuyingPrice * Quantity);
		public decimal BuyingPrice { get; set; }
		public string ItemName => $"{Product.Name} {Product.UnitOfMeasure} (Qty {Product.AvailableQuantity})";
	}
}
