using LibraryManagement.Models.Validation;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Areas.Admin.Models.ViewModels
{
    public class DashboardViewModel
    {
        [Required]
        public CompleteViewModel completeVM { get; set; }

        public IFormFile importData { get; set; }
    }
}
