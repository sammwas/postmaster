namespace PosMaster.Dal
{
	public class EmployeeLeaveCategory : BaseEntity
	{
		public string Title { get; set; }
		public int MaxDays { get; set; }
		public string AllowedGender { get; set; }
	}
}
