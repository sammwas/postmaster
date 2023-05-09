using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PosMaster.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        [Required]
        [Display(Name = "Email or Username")]
        [DataType(DataType.Text)]
        public string EmailAddress { get; set; }
        public List<AuthenticationScheme> AuthenticationSchemes { get; set; }
    }
}
