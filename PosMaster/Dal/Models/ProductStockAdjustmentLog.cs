using System;

namespace PosMaster.Dal
{
	public class ProductStockAdjustmentLog : BaseEntity
	{
		public Guid ProductId { get; set; }
		public Product Product { get; set; }
		public decimal QuantityFrom { get; set; }
		public decimal QuantityTo { get; set; }
	}
}
