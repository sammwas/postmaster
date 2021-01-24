using System;

namespace PosMaster.Dal
{
	public class Invoice : BaseEntity
	{
		public Customer Customer { get; set; }
		public Guid CustomerId { get; set; }
		public Product Product { get; set; }
		public Guid ProductId { get; set; }
		public decimal Quantity { get; set; }
		public decimal UnitPrice { get; set; }
		public Guid ReceiptId { get; set; }
		public string ReceiptNo { get; set; }
	}
}
