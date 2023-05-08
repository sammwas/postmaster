namespace PosMaster.ViewModels
{
    public class FulfillOrderViewModel : BaseViewModel
    {
        public string PaymentModeIdStr { get; set; }
        public string PaymentModeNo { get; set; }
        public string PersonnelName { get; set; }
        public decimal Amount { get; set; }
    }
}