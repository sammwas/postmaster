using System;

namespace PosMaster.Dal
{
    public class OrderLineItem : BaseEntity
    {
        public Guid OrderId { get; set; }
        public Product Product { get; set; }
        public Guid ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal TaxRate { get; set; }
        public decimal BuyingPrice { get; set; }
        public decimal Amount => SellingPrice * Quantity;
        public decimal TaxAmount => TaxRate > 0 ?
          Math.Round(TaxRate * 100 * Amount / (TaxRate * 100 + 100), 2) : 0;
        public string ItemName => $"{Product.Name} {Product.Uom} (Qty {Product.AvailableQuantity})";
    }
}
