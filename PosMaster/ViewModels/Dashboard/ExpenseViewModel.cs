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
		}

		[Required]
		[Display(Name = "Expense Type")]
		public string ExpenseTypeId { get; set; }
		public decimal Amount { get; set; }
	}
}
