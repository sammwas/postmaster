using System;

namespace PosMaster.Dal
{
	public class ProductPriceLog : BaseEntity
	{
		public Guid ProductId { get; set; }
		public Product Product { get; set; }
		public decimal PriceFrom { get; set; }
		public decimal PriceTo { get; set; }
	}
}
