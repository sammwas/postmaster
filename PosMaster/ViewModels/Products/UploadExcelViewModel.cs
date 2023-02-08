using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace PosMaster.ViewModels
{
    public class UploadExcelViewModel
    {
        public UploadExcelViewModel()
        {
            Result = new System.Collections.Generic.List<FormSelectViewModel>();
        }
        public IFormFile File { get; set; }
        [Display(Name = "Instance")]
        [Required]
        public string InstanceIdStr { get; set; }
        public System.Collections.Generic.List<FormSelectViewModel> Result { get; set; }
        public UploadExelOption Option { get; set; }
    }

    public enum UploadExelOption
    {
        Products,
        Customers
    }
}