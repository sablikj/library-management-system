using LibraryManagement.Models.Entity;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Areas.Admin.Models.ViewModels
{
    public class bookIndexViewModel
    {
        [Required]
        public IList<Book> Books { get; set; }

        [MinLength(3, ErrorMessage = "You must enter at least 3 characters!")]
        public string? SearchBook { get; set; }

        [MinLength(3, ErrorMessage = "You must enter at least 3 characters!")]
        public string? SearchAuthor { get; set; }

        public int? SearchYear { get; set; }
    }
}
