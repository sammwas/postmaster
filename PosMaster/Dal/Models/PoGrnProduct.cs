using System;

namespace PosMaster.Dal
{
	public class PoGrnProduct : BaseEntity
	{
		public Guid PoId { get; set; }
		public Guid GrnId { get; set; }
		public Guid ProductId { get; set; }
		public Product Product { get; set; }
		public decimal OrderedQuantity { get; set; }
		public decimal DeliveredQuantity { get; set; }
		public decimal UnitPrice { get; set; }
	}
}
