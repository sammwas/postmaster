using System.ComponentModel.DataAnnotations;

namespace PosMaster.ViewModels
{
    public class ApplyLicenceViewModel
    {
        [Required, Display(Name = "Email address")]
        [DataType(DataType.EmailAddress)]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        public string Licence { get; set; }
    }
}
