using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PosMaster.Dal
{
    public class TaxType : BaseEntity
    {
        public string Name { get; set; }
        public decimal Rate { get; set; }
    }
}