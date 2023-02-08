using PosMaster.Dal;
using System;
using System.Collections.Generic;

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
            Code = product.Code;
            Name = product.Name;
            PriceStartDate = product.PriceStartDate.ToString("yyyy-MM-dd");
            PriceEndDate = product.PriceEndDate.HasValue ? product.PriceEndDate.Value.ToString("yyyy-MM-dd") : "";
            SellingPrice = product.SellingPrice;
        }
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string PriceStartDate { get; set; }
        public string PriceEndDate { get; set; }
        public decimal SellingPrice { get; set; }
    }
}
