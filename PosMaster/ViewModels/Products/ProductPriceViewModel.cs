using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PosMaster.Dal;

namespace PosMaster.ViewModels
{
    public class ProductPriceViewModel : BaseViewModel
    {
        public ProductPriceViewModel()
        {
            ProductPriceMiniViewModels = new List<ProductPriceMiniViewModel>();
        }
        public List<ProductPriceMiniViewModel> ProductPriceMiniViewModels { get; set; }
    }
    public class ProductPriceMiniViewModel
    {
        public ProductPriceMiniViewModel()
        {
            
        }
        public ProductPriceMiniViewModel(Product product)
        {
            Id = product.Id;
            Name = product.Name;
            PriceStartDate = product.PriceStartDate;
            PriceEndDate = product.PriceEndDate;
            SellingPrice = product.SellingPrice;
        }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime PriceStartDate { get; set; }
        public DateTime? PriceEndDate { get; set; }
        public decimal SellingPrice { get; set; }
    }
}