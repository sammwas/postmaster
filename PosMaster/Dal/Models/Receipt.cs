using System;
using System.Collections.Generic;
using System.Linq;

namespace PosMaster.Dal
{
	public class Receipt : BaseEntity
	{
		public Receipt()
		{
			IsWalkIn = true;
			IsCredit = false;
			ReceiptLineItems = new List<ReceiptLineItem>();
		}
		public Customer Customer { get; set; }
		public Guid CustomerId { get; set; }
		public string PaymentMode { get; set; }
		public string ExternalRef { get; set; }
		public bool IsCredit { get; set; }
		public bool IsWalkIn { get; set; }
		public decimal Discount { get; set; }
		public List<ReceiptLineItem> ReceiptLineItems { get; set; }
		public decimal TotalAmount => ReceiptLineItems.Sum(i => i.Amount);
		public decimal TotalActualProfit => ReceiptLineItems.Sum(i => i.ActualProfit);
		public decimal TotalExpectedProfit => ReceiptLineItems.Sum(i => i.ExpectedProfit);
	}
}
