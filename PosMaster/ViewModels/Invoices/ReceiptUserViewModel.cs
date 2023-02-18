using PosMaster.Dal;
using System.ComponentModel.DataAnnotations;

namespace PosMaster.ViewModels
{
    public class ReceiptUserViewModel : BaseViewModel
    {
        [Display(Name = "User Type")]
        public GlUserType UserType { get; set; }
        [Display(Name = "User"), Required]
        public string UserId { get; set; }
        public decimal Amount { get; set; }
        [Display(Name = "Payment Mode"), Required]
        public string PaymentModeId { get; set; }
        [Display(Name = "Mode No."),]
        public string PaymentModeNo { get; set; }
        public decimal CreditAmount { get; set; }
        public decimal DebitAmount { get; set; }
    }
}
