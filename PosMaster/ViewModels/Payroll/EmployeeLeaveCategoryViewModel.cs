using PosMaster.Dal;

namespace PosMaster.ViewModels
{
	public class EmployeeLeaveCategoryViewModel : BaseViewModel
	{
		public EmployeeLeaveCategoryViewModel()
		{

		}

		public EmployeeLeaveCategoryViewModel(EmployeeLeaveCategory category)
		{
			Id = category.Id;
			ClientId = category.ClientId;
			InstanceId = category.InstanceId;
			Code = category.Code;
			Title = category.Title;
			MaxDays = category.MaxDays;
			FemaleOnly = category.AllowedGender.Equals("Female");
			Status = category.Status;
			IsEditMode = true;
		}

		public string Title { get; set; }
		public int MaxDays { get; set; }
		public bool FemaleOnly { get; set; }
	}
}
