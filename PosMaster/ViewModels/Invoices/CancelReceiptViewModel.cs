using System.ComponentModel.DataAnnotations;

namespace PosMaster.ViewModels
{
    public class CancelReceiptViewModel
    {
        [Required]
        public string Code { get; set; }
        public string Notes { get; set; }
        public string Personnel { get; set; }
        public decimal Amount { get; set; }
        public System.Guid ClientId { get; set; }
    }
}
