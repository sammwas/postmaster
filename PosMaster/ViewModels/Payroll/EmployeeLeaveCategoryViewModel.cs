using PosMaster.Dal;
using System.ComponentModel.DataAnnotations;

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
            Notes = category.Notes;
            Code = category.Code;
            Title = category.Title;
            MaxDays = category.MaxDays;
            FemaleOnly = category.AllowedGender.Equals("Female");
            Status = category.Status;
            IsEditMode = true;
        }

        public string Title { get; set; }
        [Display(Name = "Max Days")]
        public decimal MaxDays { get; set; }
        [Display(Name = "Female Employees Only ?")]
        public bool FemaleOnly { get; set; }
    }
}
