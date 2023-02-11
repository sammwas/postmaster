using Microsoft.AspNetCore.Http;
using PosMaster.Dal;
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
            UnitOfMeasureId = product.UnitOfMeasureId?.ToString();
            IsEditMode = true;
            IsService = product.IsService;
            Notes = product.Notes;
            TaxTypeId = product.TaxTypeId?.ToString();
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
        public string UnitOfMeasureId { get; set; }
        [Display(Name = "Instance")]
        [Required]
        public string InstanceIdStr { get; set; }
        public bool IsNewImage { get; set; }
        [Display(Name = "Tax Type")]
        public string TaxTypeId { get; set; }
        [Display(Name = "Is Service?")]
        public bool IsService { get; set; }
        public IFormFile File { get; set; }
        public string PriceStartDateStr { get; set; }
        public string PriceEndDateStr { get; set; }
        public bool IsExcelUpload { get; set; }
    }
}
