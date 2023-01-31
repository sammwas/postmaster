using System;

namespace PosMaster.Dal
{
    public class ReceiptLineItem : BaseEntity
    {
        public Guid ReceiptId { get; set; }
        public Product Product { get; set; }
        public Guid ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TaxRate { get; set; }
        public decimal BuyingPrice { get; set; }
        public decimal Discount => SellingPrice - UnitPrice;
        public decimal Amount => UnitPrice * Quantity;
        public decimal TaxAmount => TaxRate * Amount;
        public decimal NetAmount => Amount - Discount + TaxAmount;
        public decimal ActualProfit => (UnitPrice * Quantity) - (BuyingPrice * Quantity);
        public decimal ExpectedProfit => (SellingPrice * Quantity) - (BuyingPrice * Quantity);
    }
}
