using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace PosMaster.ViewModels
{
    public class ChangeUsernameViewModel
    {
        [HiddenInput]
        public string Id { get; set; }
        [Required]
        [StringLength(30, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [Display(Name = "New Username")]
        public string Username { get; set; }
        [Display(Name = "Current Username")]
        public string CurrentUsername { get; set; }
    }
}
