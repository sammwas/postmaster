using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using PosMaster.Dal;

namespace PosMaster.ViewModels
{
    public class GoodsReceivedNoteViewModel : BaseViewModel
    {
        [Required]
        [Display(Name = "Title")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Supplier")]
        public string SupplierId { get; set; }
        [Required]
        [Display(Name = "Order No.")]
        public string PoId { get; set; }
        public List<PoGrnProductViewModel> GrnItems { get; set; } = new List<PoGrnProductViewModel>();
        public GoodsReceivedNoteViewModel()
        {
        }
        public GoodsReceivedNoteViewModel(GoodReceivedNote grn)
        {
            Name = grn.Name;
            Code = grn.Code;
            SupplierId = grn.SupplierId.ToString();
            GrnItems = GetGrnProducts(grn);
            IsEditMode = true;
        }
        private List<PoGrnProductViewModel> GetGrnProducts(GoodReceivedNote goodReceivedNote)
        {
            var productViewModels = new List<PoGrnProductViewModel>();
            if (goodReceivedNote.PoGrnProducts.Any())
            {
                foreach (var item in goodReceivedNote.PoGrnProducts)
                {
                    productViewModels.Add(new PoGrnProductViewModel(item, false));
                }
            }
            return productViewModels;
        }
        public List<PoGrnProductViewModel> GetPoProducts(PurchaseOrder purchaseOrder) 
        {
            var products = new List<PoGrnProductViewModel>();
            if (purchaseOrder.PoGrnProducts.Any())
            {
                foreach (var item in purchaseOrder.PoGrnProducts)
                {
                    products.Add(new PoGrnProductViewModel(item, false));
                }
            }
            return products;
        }
    }
}