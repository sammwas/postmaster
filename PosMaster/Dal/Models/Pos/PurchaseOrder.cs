using System;
using System.Collections.Generic;
using System.Linq;

namespace PosMaster.Dal
{
	public class PurchaseOrder : BaseEntity
	{
		public PurchaseOrder()
		{
			PoGrnProducts = new List<PoGrnProduct>();
		}
		public string Name { get; set; }
		public Supplier Supplier { get; set; }
		public Guid SupplierId { get; set; }
		public List<PoGrnProduct> PoGrnProducts { get; set; }
		public decimal Amount => PoGrnProducts.Sum(p => p.PoAmount);
	}
}
