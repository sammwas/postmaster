using System;
using System.ComponentModel.DataAnnotations;

namespace PosMaster.ViewModels
{
    public class ApproveMonthlyPaymentViewModel
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public Guid ClientId { get; set; }
        [Display(Name = "Select Instance")]
        [Required]
        public Guid? InstanceId { get; set; }
        public string Personnel { get; set; }
    }
}
