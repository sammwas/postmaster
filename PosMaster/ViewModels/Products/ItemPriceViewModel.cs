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
        public DateTime PriceStartDate { get; set; } = DateTime.Now;
        public DateTime? PriceEndDate { get; set; }
        [Display(Name = "Start Date")]
        public string PriceStartDateStr => PriceStartDate.ToString("yyyy-MM-dd");
        [Display(Name = "End Date")]
        public string PriceEndDateStr => PriceEndDate.HasValue ? PriceEndDate.Value.ToString("yyyy-MM-dd") : "";
        [Display(Name = "Price")]
        public decimal SellingPrice { get; set; }
    }
}