namespace PosMaster.Dal
{
	public class EmployeeSalaryLog : BaseEntity
	{
		public string UserId { get; set; }
		public User User { get; set; }
		public decimal SalaryFrom { get; set; }
		public decimal SalaryTo { get; set; }
	}
}
