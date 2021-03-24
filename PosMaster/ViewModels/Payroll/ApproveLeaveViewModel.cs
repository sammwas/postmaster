using PosMaster.Dal;
using System;

namespace PosMaster.ViewModels
{
	public class ApproveLeaveViewModel
	{
		public string Personnel { get; set; }
		public string Comment { get; set; }
		public ApplicationStatus Status { get; set; }
		public Guid Id { get; set; }
	}
}
