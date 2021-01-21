namespace PosMaster.Dal
{
	public class Expense : BaseEntity
	{
		public string ExpenseType { get; set; }
		public decimal Amount { get; set; }
		public bool IsLimited => Amount > 0;
	}
}
