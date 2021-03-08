using System;

namespace PosMaster.Dal
{
	public class Invoice : BaseEntity
	{
		public Receipt Receipt { get; set; }
		public Guid ReceiptId { get; set; }
		public decimal TaxAmount { get; set; }
		public decimal TotalAmount => Receipt.TotalAmount;
		public string DueDate => DateCreated.AddMonths(1).ToString("dd-MMM-yyyy");
	}
}
