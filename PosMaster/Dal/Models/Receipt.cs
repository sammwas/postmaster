﻿using System;

namespace PosMaster.Dal
{
	public class Receipt : BaseEntity
	{
		public Receipt()
		{
			IsWalkIn = true;
		}
		public decimal UnitPrice { get; set; }
		public decimal Discount { get; set; }
		public Customer Customer { get; set; }
		public Product Product { get; set; }
		public Guid ProductId { get; set; }
		public decimal Quantity { get; set; }
		public decimal Amount => (UnitPrice * Quantity) - Discount;
		public string PaymentMode { get; set; }
		public string ExternalRef { get; set; }
		public bool IsCredit { get; set; }
		public bool IsWalkIn { get; set; }
	}
}
