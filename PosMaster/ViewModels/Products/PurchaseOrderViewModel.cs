using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using PosMaster.Dal;

namespace PosMaster.ViewModels
{
    public class PurchaseOrderViewModel : BaseViewModel
    {
        [Required]
        [Display(Name = "Purchase Order Title")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Supplier")]
        public string SupplierId { get; set; }
        public List<PoGrnProductViewModel> PurchaseOrderItems { get; set; } = new List<PoGrnProductViewModel>();
        public PurchaseOrderViewModel()
        {
        }
        public PurchaseOrderViewModel(PurchaseOrder purchaseOrder)
        {
            Name = purchaseOrder.Name;
            Code = purchaseOrder.Code;
            SupplierId = purchaseOrder.SupplierId.ToString();
            PurchaseOrderItems = GetPoProducts(purchaseOrder);
            IsEditMode = true;
        }
        private List<PoGrnProductViewModel> GetPoProducts(PurchaseOrder po)
        {
            var productViewModels = new List<PoGrnProductViewModel>();
            if (po.PoGrnProducts.Any())
            {
                foreach (var item in po.PoGrnProducts)
                {
                    productViewModels.Add(new PoGrnProductViewModel(item, true));
                }
            }
            return productViewModels;
        }
    }
}
