using System.ComponentModel.DataAnnotations;

namespace PosMaster.ViewModels
{
    public class TestEmailViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Recipient { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public string Content { get; set; }
        public bool IsTest { get; set; }
    }
}
