namespace PosMaster.Dal
{
	public class EmployeeSalary : BaseEntity
	{
		public string UserId { get; set; }
		public decimal BasicPay { get; set; }
		public decimal Allowance { get; set; }
		public decimal Deduction { get; set; }
		public decimal NetAmount => BasicPay + Allowance - Deduction;
	}
}
