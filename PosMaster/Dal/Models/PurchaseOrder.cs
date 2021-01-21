using System;

namespace PosMaster.Dal
{
	public class PurchaseOrder : BaseEntity
	{
		public string Name { get; set; }
		public Supplier Supplier { get; set; }
		public Guid SupplierId { get; set; }
	}
}
