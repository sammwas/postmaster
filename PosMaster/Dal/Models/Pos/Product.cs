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
        public decimal TaxRate => TaxType == null ? 0 : TaxType.Rate;
        public DateTime PriceStartDate { get; set; }
        public DateTime? PriceEndDate { get; set; }
        public bool IsService { get; set; }
        public string ProductInstanceStamp { get; set; }
        public Guid? TaxTypeId { get; set; }
        public TaxType TaxType { get; set; }
        public decimal TotalValue => AvailableQuantity * SellingPrice;
        public string ProductUniqueCode => $"{InstanceId}.{Code}";
        public bool ShowSellingPrice => SellingPrice > 0 && PriceStartDate.Date <= DateTime.Now.Date
            && (!PriceEndDate.HasValue || PriceEndDate.Value.Date >= DateTime.Now.Date);

    }
}
