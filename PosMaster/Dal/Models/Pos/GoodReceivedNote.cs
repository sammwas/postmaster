using System;
using System.Collections.Generic;

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
        public GoodReceivedNote()
        {
            PoGrnProducts = new List<PoGrnProduct>();
        }
    }
}
