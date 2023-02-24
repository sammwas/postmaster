using PosMaster.Dal;

namespace PosMaster.ViewModels
{
    public class CustomerBalanceViewModel : BaseViewModel
    {
        public Customer Customer { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
    }
}
