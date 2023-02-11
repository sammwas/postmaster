using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PosMaster.ViewModels
{
    public class ItemPriceViewModel : BaseViewModel
    {
        [Required]
        [Display(Name = "Product")]
        public string ProductId { get; set; }
        [Display(Name = "Start Date")]
        public DateTime PriceStartDate { get; set; } = DateTime.Now;
        [Display(Name = "End Date")]
        public DateTime? PriceEndDate { get; set; }
        public string PriceStartDateStr => PriceStartDate.ToString("yyyy-MM-dd");
        public string PriceEndDateStr => PriceEndDate.HasValue ? PriceEndDate.Value.ToString("yyyy-MM-dd") : "";
        [Display(Name = "Price")]
        public decimal SellingPrice { get; set; }
    }
}