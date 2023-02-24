using System.ComponentModel.DataAnnotations;

namespace PosMaster.ViewModels
{
    public class ProductSaleViewModel : BaseViewModel
    {
        [Required]
        [Display(Name = "Customer")]
        public string CustomerId { get; set; }
        [Display(Name = "Payment Mode")]
        public string PaymentModeIdStr { get; set; }
        [Display(Name = "Payment Mode No.")]
        public string PaymentModeNo { get; set; }
        [Display(Name = "Amount Received")]
        public decimal AmountReceived { get; set; }
        public string PinNo { get; set; }
        public bool IsCredit { get; set; }
        public bool IsWalkIn { get; set; }
        public string LineItemListStr { get; set; }
        public string PersonnelName { get; set; }
    }
}
