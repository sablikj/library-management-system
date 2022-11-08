using LibraryManagement.Models.Entity;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Areas.Admin.Models.ViewModels
{
    public class UserIndexViewModel
    {
        [Required]
        public IList<User> Users { get; set; }

        [MinLength(3, ErrorMessage = "You must enter at least 3 characters!")]
        public string? SearchName { get; set; }

        [MinLength(3, ErrorMessage = "You must enter at least 3 characters!")]
        public string? SearchSurname { get; set; }

        [MinLength(3, ErrorMessage = "You must enter at least 3 characters!")]
        public string? SearchStreet { get; set; }        
        public int SearchHouseNumber { get; set; }

        [MinLength(3, ErrorMessage = "You must enter at least 3 characters!")]
        public string? SearchCity { get; set; }
        
        public int? SearchZip { get; set; }
        
        public int? SearchSSN { get; set; }
    }
}
