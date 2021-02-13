using System;
using System.Collections.Generic;

namespace PosMaster.ViewModels
{
	public class SuperAdminDashboardViewModel
	{
        public SuperAdminDashboardViewModel()
        {
			DailySalesList = new List<ChartData>();
        }
		public int Clients { get; set; }
		public int ClientInstances { get; set; }
		public int Users { get; set; }
		public int Products { get; set; }
		public decimal TotalStockValue { get; set; }
		public decimal TotalReceiptsAmount { get; set; }
		public decimal TotalExpectedProfit { get; set; }
		public decimal TotalActualProfit { get; set; }
        public List<ChartData> DailySalesList { get; set; }
    }

	public class ChartData 
	{
        public string Day { get; set; }
        public decimal Amount { get; set; }
    }
}
