using System.Collections.Generic;

namespace PosMaster.ViewModels
{
    public class CloseOfDayViewModel
    {
        public CloseOfDayViewModel()
        {
            SalesByClerk = ReceiptsByClerk = PaymentsByMode
                = new List<KeyAmountViewModel>();
        }
        public string Day { get; set; }
        public decimal TotalSale { get; set; }
        public decimal CreditSale { get; set; }
        public decimal CashSale => TotalSale - CreditSale;
        public List<KeyAmountViewModel> SalesByClerk { get; set; }
        public int InvoiceCustomerServed { get; set; }
        public decimal TotalRepayment { get; set; }
        public List<KeyAmountViewModel> ReceiptsByClerk { get; set; }

        public decimal DailyCashReturn => CashSale + TotalRepayment;
        public List<KeyAmountViewModel> PaymentsByMode { get; set; }
    }

    public class KeyAmountViewModel
    {
        public string Key { get; set; }
        public decimal Amount { get; set; }
    }
}
