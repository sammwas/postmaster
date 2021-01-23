using PosMaster.Dal;
using System.ComponentModel.DataAnnotations;

namespace PosMaster.ViewModels
{
	public class ExpenseTypeViewModel : BaseViewModel
	{
		public ExpenseTypeViewModel()
		{

		}

		public ExpenseTypeViewModel(ExpenseType type)
		{
			Id = type.Id;
			ClientId = type.ClientId;
			InstanceId = type.InstanceId;
			Notes = type.Notes;
			Status = type.Status;
			Name = type.Name;
			IsEditMode = true;
			MaxApprovedAmount = type.MaxApprovedAmount;
		}

		[Required]
		public string Name { get; set; }
		[Display(Name = "Max Approved Amount")]
		public decimal MaxApprovedAmount { get; set; }
	}
}
