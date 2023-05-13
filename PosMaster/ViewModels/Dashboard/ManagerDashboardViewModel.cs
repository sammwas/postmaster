namespace PosMaster.ViewModels
{
    public class ManagerDashboardViewModel
    {
        public decimal TodaySales { get; set; }
        public decimal TodayRepayments { get; set; }
        public decimal WeeklySales { get; set; }
        public decimal MonthlySales { get; set; }
        public decimal TotalStockValue { get; set; }
        public decimal TotalReceiptsAmount { get; set; }
        public decimal TotalExpectedProfit { get; set; }
        public decimal TotalActualProfit { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal SupplierExpenses { get; set; }
        public decimal ProfitExpenses => TotalExpenses - SupplierExpenses;
    }
}
