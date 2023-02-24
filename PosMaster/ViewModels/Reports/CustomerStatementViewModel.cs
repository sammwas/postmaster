using PosMaster.Dal;

namespace PosMaster.ViewModels
{
    public class CustomerStatementViewModel
    {
        public Customer Customer { get; set; }
        public System.Collections.Generic.List<GeneralLedgerEntry > LedgerEntries { get; set; }
    }
}
