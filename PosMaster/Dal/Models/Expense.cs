using System;

namespace PosMaster.Dal
{
    public class Expense : BaseEntity
    {
        public Guid ExpenseTypeId { get; set; }
        public ExpenseType ExpenseType { get; set; }
        public decimal Amount { get; set; }
        public PaymentMode PaymentMode { get; set; }
        public Guid? PaymentModeId { get; set; }
        public string ModeNumber { get; set; }
        public Supplier Supplier { get; set; }
        public Guid? SupplierId { get; set; }
    }
}
