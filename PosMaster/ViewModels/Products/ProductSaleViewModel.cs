using System.ComponentModel.DataAnnotations;

namespace PosMaster.ViewModels
{
    public class ProductSaleViewModel : BaseViewModel
    {
        [Required]
        [Display(Name = "Customer")]
        public string CustomerId { get; set; }
        [Display(Name = "Payment Mode")]
        public string PaymentMode { get; set; }
        [Display(Name = "External Ref")]
        public string ExternalRef { get; set; }
        public string KraPin { get; set; }
        public bool IsCredit { get; set; }
        public bool IsWalkIn { get; set; }
        public string LineItemListStr { get; set; }
    }
}
