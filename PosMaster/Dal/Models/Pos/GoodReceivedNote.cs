﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace PosMaster.Dal
{
    public class GoodReceivedNote : BaseEntity
    {
        public Guid PoId { get; set; }
        public string PoCode { get; set; }
        public string Name { get; set; }
        public Supplier Supplier { get; set; }
        public Guid SupplierId { get; set; }
        public List<PoGrnProduct> PoGrnProducts { get; set; }
        public decimal Amount => PoGrnProducts.Sum(p => p.GrnAmount);
        public decimal AmountReceived { get; set; }
        public decimal Balance => Amount - AmountReceived;
        public GoodReceivedNote()
        {
            PoGrnProducts = new List<PoGrnProduct>();
        }
    }
}
