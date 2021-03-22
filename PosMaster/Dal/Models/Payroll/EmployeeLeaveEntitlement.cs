using System;

namespace PosMaster.Dal
{
	public class EmployeeLeaveEntitlement : BaseEntity
	{
		public string UserId { get; set; }
		public User User { get; set; }
		public Guid EmployeeLeaveCategoryId { get; set; }
		public EmployeeLeaveCategory EmployeeLeaveCategory { get; set; }
		public int RemainingDays { get; set; }
	}
}
