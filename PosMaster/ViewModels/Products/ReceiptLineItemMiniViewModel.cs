using System;

namespace PosMaster.ViewModels
{
    public class ReceiptLineItemMiniViewModel
    {
        public Guid ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
