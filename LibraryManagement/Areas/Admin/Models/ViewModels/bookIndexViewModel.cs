using LibraryManagement.Models.Entity;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Areas.Admin.Models.ViewModels
{
    public class bookIndexViewModel
    {
        [Required]
        public IList<Book> Books { get; set; }
        public string SearchBook { get; set; }
        public string SearchAuthor { get; set; }
        public int SearchYear { get; set; }
    }
}
