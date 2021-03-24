using System;

namespace PosMaster.ViewModels
{
	public class ApproveMonthlyPaymentViewModel
	{
		public int Month { get; set; }
		public int Year { get; set; }
		public Guid? ClientId { get; set; }
		public Guid? InstanceId { get; set; }
		public string Personnel { get; set; }
	}
}
