using System;

namespace PosMaster.Dal
{
	public class Expense : BaseEntity
	{
		public Guid ExpenseTypeId { get; set; }
		public ExpenseType ExpenseType { get; set; }
		public decimal Amount { get; set; }
		public bool IsLimited => Amount > 0;
	}
}
