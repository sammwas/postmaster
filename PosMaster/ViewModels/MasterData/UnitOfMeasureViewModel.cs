using PosMaster.Dal;
using System.ComponentModel.DataAnnotations;

namespace PosMaster.ViewModels
{
	public class UnitOfMeasureViewModel : BaseViewModel
	{
		public UnitOfMeasureViewModel()
		{

		}

		public UnitOfMeasureViewModel(UnitOfMeasure unit)
		{
			Id = unit.Id;
			ClientId = unit.ClientId;
			InstanceId = unit.InstanceId;
			Notes = unit.Notes;
			Status = unit.Status;
			Name = unit.Name;
			IsEditMode = true;
		}
		[Required]
		public string Name { get; set; }
	}
}
