using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PosMaster.Dal;

namespace PosMaster.ViewModels
{
    public class ItemViewModel
    {
        public ItemViewModel()
        {
        }

        public ItemViewModel(Item item)
        {
            Id = item.Id;
            ClientId = item.ClientId;
            InstancesId = item.InstancesId;
            Code = item.Code;
            Status = item.Status;
            ProductCategoryId = item.ProductCategoryId.ToString();
            Name = item.Name;
            ImagePath = item.ImagePath;
            AllowDiscount = item.AllowDiscount;
            ReorderLevel = item.ReorderLevel;
            UnitOfMeasure = item.UnitOfMeasure;
            IsEditMode = true;
            Notes = item.Notes;
            InstancesIdStr = GetInstances(item);
            ItemNature = item.ItemNature;
        }
        [HiddenInput]
        public Guid Id { get; set; }
        [HiddenInput]
        public Guid ClientId { get; set; }
        public List<Guid> InstancesId { get; set; } = new List<Guid>();
        public string ErrorMessage { get; set; }
        public string Message { get; set; }
        public string Notes { get; set; }
        public bool Success { get; set; }
        [HiddenInput]
        public bool IsEditMode { get; set; }
        public EntityStatus Status { get; set; }
        public string Personnel { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        [Display(Name = "Product Category")]
        public string ProductCategoryId { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
        [Display(Name = "Allow Discount")]
        public bool AllowDiscount { get; set; }
        [Display(Name = "Reorder Level")]
        public decimal ReorderLevel { get; set; }
        [Display(Name = "Unit of Measure")]
        public string UnitOfMeasure { get; set; }
        [Display(Name = "Instances")]
        [Required]
        public List<string> InstancesIdStr { get; set; } = new List<string>();
        public bool IsNewImage { get; set; }
        public IFormFile File { get; set; }
        [Display(Name = "Item Type")]
        public ItemNature ItemNature { get; set; }
        public List<string> GetInstances(Item item)
        {
            var instances = new List<string>();
            if (!item.InstancesId.Any())
                return instances;
            foreach (var i in item.InstancesId)
            {
                instances.Add(i.ToString());
            }
            return instances;
        }
    }
}