using System.ComponentModel.DataAnnotations;

namespace PosMaster.ViewModels
{
    public class ApplyLicenceViewModel
    {
        [Required, Display(Name = "UserName")]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        public string Licence { get; set; }
    }
}
