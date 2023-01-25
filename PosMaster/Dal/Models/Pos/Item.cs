using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PosMaster.Dal
{
    public class Item
    {
        public Item()
        {
            DateCreated = DateTime.Now;
            Status = EntityStatus.Active;
            InstancesId = new List<Guid>();
        }

        public Guid Id { get; set; }
        public string Code { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateLastModified { get; set; }
        public Guid ClientId { get; set; }
        public List<Guid> InstancesId { get; set; }
        public string Personnel { get; set; }
        public string LastModifiedBy { get; set; }
        public string Notes { get; set; }
        public EntityStatus Status { get; set; }
        public Guid ProductCategoryId { get; set; }
        public ProductCategory ProductCategory { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public bool AllowDiscount { get; set; }
        public decimal ReorderLevel { get; set; }
        public string UnitOfMeasure { get; set; }
        public ItemNature ItemNature { get; set; }
        [DisplayFormat(NullDisplayText = "N/A")]
        public Guid? TaxTypeId { get; set; }
        public TaxType TaxType { get; set; }
        public decimal BuyingPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal AvailableQuantity { get; set; }
        public bool IsTaxable => TaxType != null;
    }
}