using PosMaster.Dal;
using System.ComponentModel.DataAnnotations;

namespace PosMaster.ViewModels
{
	public class ProductCategoryViewModel : BaseViewModel
	{
		public ProductCategoryViewModel()
		{

		}

		public ProductCategoryViewModel(ProductCategory category)
		{
			Id = category.Id;
			Code = category.Code;
			Name = category.Name;
			ImagePath = category.ImagePath;
			ClientId = category.ClientId;
			InstanceId = category.InstanceId;
			IsEditMode = true;
			Notes = category.Notes;
			Status = category.Status;
		}

		[Required]
		public string Name { get; set; }
		public string ImagePath { get; set; }
		public bool IsNewImage { get; set; }
	}
}
