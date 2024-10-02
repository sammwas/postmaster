using PosMaster.Dal;
using System.ComponentModel.DataAnnotations;

namespace PosMaster.ViewModels
{
    public class ExpenseViewModel : BaseViewModel
    {
        public ExpenseViewModel()
        {

        }

        public ExpenseViewModel(Expense expense)
        {
            Code = expense.Code;
            Id = expense.Id;
            ClientId = expense.ClientId;
            InstanceId = expense.InstanceId;
            Notes = expense.Notes;
            Status = expense.Status;
            Amount = expense.Amount;
            ExpenseTypeId = expense.ExpenseTypeId.ToString();
            IsEditMode = true;
            ModeNumber = expense.ModeNumber;
            SupplierId = expense.SupplierId?.ToString();
            PaymentModeId = expense.PaymentModeId?.ToString();
        }

        [Required]
        [Display(Name = "Expense Type")]
        public string ExpenseTypeId { get; set; }
        public decimal Amount { get; set; }
        [Display(Name = "Mode Number")]
        public string ModeNumber { get; set; }
        [Required]
        [Display(Name = "Payment Mode")]
        public string PaymentModeId { get; set; }
        [Display(Name = "Supplier")]
        public string SupplierId { get; set; }
        public System.Guid GrnId { get; set; }
    }
}
