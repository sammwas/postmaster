using System;

namespace PosMaster.Dal
{
	public class EmployeeMonthPayment : BaseEntity
	{
		public string UserId { get; set; }
		public User User { get; set; }
		public decimal BasicPay { get; set; }
		public decimal Allowance { get; set; }
		public decimal Deduction { get; set; }
		public int Year { get; set; }
		public int Month { get; set; }
		public DateTime? DateApproved { get; set; }
	}
}
