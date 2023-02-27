using PosMaster.Dal;

namespace PosMaster.ViewModels
{
    public class CustomerStatementViewModel
    {
        public CustomerStatementViewModel()
        {
            LedgerEntries = new System.Collections.Generic.List<GeneralLedgerEntry>();
        }
        public Customer Customer { get; set; }
        public System.Collections.Generic.List<GeneralLedgerEntry> LedgerEntries { get; set; }
    }
}
