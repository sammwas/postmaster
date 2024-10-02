using PosMaster.Dal;
using System;

namespace PosMaster.ViewModels
{
    public class TopSellingProductViewModel
    {
        public Product Product { get; set; }
        public Guid ProductId { get; set; }
        public decimal Volume { get; set; }
        public decimal Amount { get; set; }
        public Guid InstanceId { get; set; }
        public Guid ClientId { get; set; }
        public string Personnel { get; set; }
        public string ClerkNames { get; set; }
    }
}
