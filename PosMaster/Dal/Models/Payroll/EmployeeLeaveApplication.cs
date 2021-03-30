using System;

namespace PosMaster.Dal
{
    public class EmployeeLeaveApplication : BaseEntity
    {
        public ApplicationStatus ApplicationStatus { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public DateTime DateFrom { get; set; }
        public string DateFromStr => DateFrom.ToString("dd-MMM-yyyy");
        public DateTime DateTo { get; set; }
        public string DateToStr => DateTo.ToString("dd-MMM-yyyy");
        public EmployeeLeaveCategory EmployeeLeaveCategory { get; set; }
        public Guid EmployeeLeaveCategoryId { get; set; }
        public decimal Days { get; set; }
        public string Comments { get; set; }
    }
}
