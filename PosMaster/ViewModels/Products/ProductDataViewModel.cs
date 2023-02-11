using PosMaster.Dal;
using System.Collections.Generic;

namespace PosMaster.ViewModels
{
    public class ProductDataViewModel : BaseViewModel
    {
        public ProductDataViewModel()
        {
            ProductPriceLogs = new List<ProductPriceLog>();
            ProductStockAdjustmentLogs = new List<ProductStockAdjustmentLog>();
            Product = new Product();
        }
        public Product Product { get; set; }
        public List<ProductPriceLog> ProductPriceLogs { get; set; }
        public List<ProductStockAdjustmentLog> ProductStockAdjustmentLogs { get; set; }
        public decimal TotalQtySold { get; set; }
        public decimal TotalSales { get; set; }
    }
}
