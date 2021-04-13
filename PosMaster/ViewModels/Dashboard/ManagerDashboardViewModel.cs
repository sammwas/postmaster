namespace PosMaster.ViewModels
{
	public class ManagerDashboardViewModel
	{
		public int Products { get; set; }
		public decimal TodaySales { get; set; }
		public decimal WeeklySales { get; set; }
		public decimal MonthlySales { get; set; }
		public decimal TotalStockValue { get; set; }
		public decimal TotalReceiptsAmount { get; set; }
		public decimal TotalExpectedProfit { get; set; }
		public decimal TotalActualProfit { get; set; }
        public int TotalUsers { get; set; }
    }
}
