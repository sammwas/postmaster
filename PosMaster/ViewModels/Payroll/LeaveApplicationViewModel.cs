using System.ComponentModel.DataAnnotations;

namespace PosMaster.ViewModels
{
	public class LeaveApplicationViewModel : BaseViewModel
	{
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
		public int Days { get; set; }
		public string Gender { get; set; }
	}
}
