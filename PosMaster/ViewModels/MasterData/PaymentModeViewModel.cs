using PosMaster.Dal;
using System.ComponentModel.DataAnnotations;

namespace PosMaster.ViewModels
{
	public class PaymentModeViewModel : BaseViewModel
	{
		public PaymentModeViewModel()
		{

		}

		public PaymentModeViewModel(PaymentMode mode)
		{
			Id = mode.Id;
			Code = mode.Code;
			Name = mode.Name;
			ImagePath = mode.ImagePath;
			ClientId = mode.ClientId;
			InstanceId = mode.InstanceId;
			IsEditMode = true;
			Notes = mode.Notes;
			Status = mode.Status;
		}

		[Required]
		public string Name { get; set; }
		public string ImagePath { get; set; }
		public bool IsNewImage { get; set; }
	}
}
