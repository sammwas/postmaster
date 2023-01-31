using System;

namespace PosMaster.Dal
{
    public class PoGrnProduct : BaseEntity
    {
        public Guid PurchaseOrderId { get; set; }
        public Guid GoodReceivedNoteId { get; set; }
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
        public decimal PoQuantity { get; set; }
        public decimal GrnQuantity { get; set; }
        public decimal PoUnitPrice { get; set; }
        public decimal GrnUnitPrice { get; set; }
        public string PoNotes { get; set; }
        public string GrnNotes { get; set; }
        public decimal PoAmount => PoUnitPrice * PoQuantity;
        public decimal GrnAmount => GrnUnitPrice * GrnQuantity;
    }
}
