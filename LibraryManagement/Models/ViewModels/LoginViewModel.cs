using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }

        public bool LoginFailed { get; set; }

        public string? ReturnUrl { get; set; } // Temp fix
    }
}
