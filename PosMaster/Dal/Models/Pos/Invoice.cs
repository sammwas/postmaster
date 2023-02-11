using System;

namespace PosMaster.Dal
{
    public class Invoice : BaseEntity
    {
        public Receipt Receipt { get; set; }
        public Guid ReceiptId { get; set; }
        public decimal TotalAmount => Receipt.Amount;
        public string DueDate => DateCreated.AddMonths(1).ToString("dd-MMM-yyyy");
    }
}
