using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace PosMaster.ViewModels
{
    public class ProductViewModel : BaseViewModel
	{
		public ProductViewModel()
		{

		}

		public ProductViewModel(Product product)
		{
			Id = product.Id;
			ClientId = product.ClientId;
			InstanceId = product.InstanceId;
			Code = product.Code;
			Status = product.Status;
			ProductCategoryId = product.ProductCategoryId.ToString();
			Name = product.Name;
			ImagePath = product.ImagePath;
			AllowDiscount = product.AllowDiscount;
			BuyingPrice = product.BuyingPrice;
			SellingPrice = product.SellingPrice;
			ReorderLevel = product.ReorderLevel;
			AvailableQuantity = product.AvailableQuantity;
			UnitOfMeasure = product.UnitOfMeasure;
			IsEditMode = true;
			Notes = product.Notes;
			InstanceIdStr = product.InstanceId.ToString();
		}

		[Required]
		[Display(Name = "Product Category")]
		public string ProductCategoryId { get; set; }
		public string Name { get; set; }
		public string ImagePath { get; set; }
		[Display(Name = "Allow Discount")]
		public bool AllowDiscount { get; set; }
		[Display(Name = "Buying Price")]
		public decimal BuyingPrice { get; set; }
		[Display(Name = "Selling Price")]
		public decimal SellingPrice { get; set; }
		[Display(Name = "Reorder Level")]
		public decimal ReorderLevel { get; set; }
		[Display(Name = "Available Quantity")]
		public decimal AvailableQuantity { get; set; }
		[Display(Name = "Unit of Measure")]
		[Required]
		public string UnitOfMeasure { get; set; }
		[Display(Name = "Instance")]
		[Required]
		public string InstanceIdStr { get; set; }
		public bool IsNewImage { get; set; }
		public IFormFile File { get; set; }
	}
}
