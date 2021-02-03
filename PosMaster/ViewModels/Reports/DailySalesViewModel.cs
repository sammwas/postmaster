using System;

namespace PosMaster.ViewModels
{
    public class DailySalesViewModel
    {
        public string DayStr => Day.ToString("dd");
        public DateTime Day { get; set; }
        public decimal TotalSales { get; set; }
        public decimal ExpectedProfit { get; set; }
        public decimal ActualProfit { get; set; }
    }
}
