using System;

namespace PosMaster.Dal
{
	public class Invoice : BaseEntity
	{
		public Receipt Receipt { get; set; }
		public Guid CustomerId { get; set; }
		public Guid ReceiptId { get; set; }
		public string ReceiptNo { get; set; }
		public decimal TaxAmount { get; set; }
	}
}
