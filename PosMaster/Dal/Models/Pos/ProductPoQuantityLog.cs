using System;

namespace PosMaster.Dal
{
    public class ProductPoQuantityLog : BaseEntity
    {
        public Guid ProductId { get; set; }
        public Guid PurchaseOrderId { get; set; }
        public Product Product { get; set; }
        public PurchaseOrder PurchaseOrder { get; set; }
        public decimal BuyingPrice { get; set; }
        public decimal DeliveredQuantity { get; set; }
        public decimal AvailableQuantity { get; set; }
    }
}
