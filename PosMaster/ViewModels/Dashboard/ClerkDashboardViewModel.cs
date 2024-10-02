namespace PosMaster.ViewModels
{
    public class ClerkDashboardViewModel
    {
        public decimal TodayExpenses { get; set; }
        public decimal TodaySales { get; set; }
        public decimal CashSales { get; set; }
        public decimal CreditSales => TodaySales - CashSales;
    }
}
