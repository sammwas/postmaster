using System;
using System.ComponentModel.DataAnnotations;

namespace PosMaster.ViewModels
{
    public class ItemPriceViewModel : BaseViewModel
    {
        public ItemPriceViewModel()
        {

        }
        public ItemPriceViewModel(Dal.Product product)
        {
            ProductId = product.Id.ToString();
            PriceStartDateStr = product.PriceStartDateStr;
            PriceEndDateStr = product.PriceEndDateStr;
            SellingPrice = product.SellingPrice;
        }

        [Required]
        [Display(Name = "Product")]
        public string ProductId { get; set; }
        [Required, Display(Name = "Start Date")]
        public string PriceStartDateStr { get; set; }
        [Display(Name = "End Date (optional)")]
        public string PriceEndDateStr { get; set; }
        [Display(Name = "Price")]
        public decimal SellingPrice { get; set; }
    }
}