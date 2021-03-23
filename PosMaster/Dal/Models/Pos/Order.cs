using System;
using System.Collections.Generic;
using System.Linq;

namespace PosMaster.Dal
{
	public class Order : BaseEntity
	{
		public Order()
		{
			OrderLineItems = new List<OrderLineItem>();
		}
		public string Name { get; set; }
		public Customer Customer { get; set; }
		public Guid CustomerId { get; set; }
		public List<OrderLineItem> OrderLineItems { get; set; }
		public decimal TotalAmount => OrderLineItems.Sum(i => i.Amount);
	}
}
