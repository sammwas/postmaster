using System;
using System.ComponentModel.DataAnnotations;

namespace PosMaster.ViewModels
{
    public class ApproveMonthlyPaymentViewModel
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public Guid ClientId { get; set; }
        [Display(Name = "Instance"), Required]
        public Guid? InstanceId { get; set; }
        public string Personnel { get; set; }
        public string Notes { get; set; }
        [Display(Name = "Expense Type"), Required]
        public string ExpenseTypeId { get; set; }
        [Display(Name = "Payment Mode"), Required]
        public string PaymentModeId { get; set; }
        [Display(Name = "Mode Number")]
        public string PaymentModeNo { get; set; }
    }
}
