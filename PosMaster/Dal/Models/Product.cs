using System;

namespace PosMaster.Dal
{
	public class Product : BaseEntity
	{
		public Guid ProductCategoryId { get; set; }
		public ProductCategory ProductCategory { get; set; }
		public string Name { get; set; }
		public string ImagePath { get; set; }
		public bool AllowDiscount { get; set; }
		public decimal BuyingPrice { get; set; }
		public decimal SellingPrice { get; set; }
		public decimal ReorderLevel { get; set; }
		public decimal AvailableQuantity { get; set; }
		public string UnitOfMeasure { get; set; }
	}
}
