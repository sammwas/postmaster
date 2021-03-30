using PosMaster.Dal;
using System.ComponentModel.DataAnnotations;

namespace PosMaster.ViewModels
{
    public class LeaveApplicationViewModel : BaseViewModel
    {
        public LeaveApplicationViewModel()
        {

        }

        public LeaveApplicationViewModel(EmployeeLeaveApplication leave)
        {
            Id = leave.Id;
            IsEditMode = true;
            UserId = leave.UserId;
            DateFrom = leave.DateFromStr;
            DateTo = leave.DateToStr;
            EmployeeLeaveCategoryId = leave.EmployeeLeaveCategoryId.ToString();
            Days = leave.Days;
            Notes = leave.Notes;
        }
        public string UserId { get; set; }
        [Required]
        [Display(Name = "From")]
        public string DateFrom { get; set; }
        [Required]
        [Display(Name = "To")]
        public string DateTo { get; set; }
        [Required]
        [Display(Name = "Leave Category")]
        public string EmployeeLeaveCategoryId { get; set; }
        public decimal Days { get; set; }
        public string Gender { get; set; }
    }
}
