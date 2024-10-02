namespace PosMaster.ViewModels
{
    public class RepaymentQueryableViewModel
    {
        public System.Guid DocumentId { get; set; }
        public System.DateTime DateCreated { get; set; }
        public string Personnel { get; set; }
        public decimal Amount { get; set; }
    }
}
