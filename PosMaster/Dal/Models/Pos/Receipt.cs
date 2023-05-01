using System;
using System.Collections.Generic;
using System.Linq;

namespace PosMaster.Dal
{
    public class Receipt : BaseEntity
    {
        public Receipt()
        {
            IsWalkIn = true;
            IsCredit = false;
            ReceiptLineItems = new List<ReceiptLineItem>();
            Customer = new Customer();
        }
        public Customer Customer { get; set; }
        public Guid CustomerId { get; set; }
        public Guid? PaymentModeId { get; set; }
        public PaymentMode PaymentMode { get; set; }
        public string PaymentModeNo { get; set; }
        public bool IsCredit { get; set; }
        public bool IsWalkIn { get; set; }
        public decimal AmountReceived { get; set; }
        public string PinNo { get; set; }
        public bool IsPrinted { get; set; }
        public int PrintCount { get; set; }
        public string PersonnelName { get; set; }
        public string Stamp { get; set; }
        public GlUserType UserType { get; set; }
        public List<ReceiptLineItem> ReceiptLineItems { get; set; }
        public decimal Discount => ReceiptLineItems.Sum(i => i.Discount);
        public decimal Tax => ReceiptLineItems.Sum(i => i.TaxAmount);
        public decimal Amount => ReceiptLineItems.Sum(i => i.Amount);
        public decimal Balance => Amount - AmountReceived;
    }
}
