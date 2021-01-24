using System;

namespace PosMaster.Dal
{
	public class Receipt : BaseEntity
	{
		public Receipt()
		{
			IsWalkIn = true;
			IsCredit = false;
		}
		public decimal UnitPrice { get; set; }
		public decimal Discount { get; set; }
		public Customer Customer { get; set; }
		public Guid CustomerId { get; set; }
		public Product Product { get; set; }
		public Guid ProductId { get; set; }
		public decimal Quantity { get; set; }
		public decimal Amount => (UnitPrice * Quantity) + TaxAmount - Discount;
		public string PaymentMode { get; set; }
		public string ExternalRef { get; set; }
		public bool IsCredit { get; set; }
		public bool IsWalkIn { get; set; }
		public decimal TaxAmount { get; set; }
	}
}
