using System.ComponentModel.DataAnnotations;

namespace PosMaster.ViewModels
{
    public class ProductStockAdjustmentViewModel : BaseViewModel
    {
        [Required]
        [Display(Name = "Product")]
        public string ProductId { get; set; }
        [Display(Name = "Final Qty")]
        public decimal QuantityTo { get; set; }
        [Display(Name = "Final Buy Price")]
        public decimal BuyingPriceTo { get; set; }
    }
}
