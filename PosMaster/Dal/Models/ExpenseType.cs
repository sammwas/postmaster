namespace PosMaster.Dal
{
	public class ExpenseType : BaseEntity
	{
		public string Name { get; set; }
		public decimal MaxApprovedAmount { get; set; }
	}
}
