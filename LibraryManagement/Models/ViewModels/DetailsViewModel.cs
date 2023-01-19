using LibraryManagement.Models.Entity;

namespace LibraryManagement.Models.ViewModels
{
    public class DetailsViewModel
    {
        public Book book { get; set; }

        public bool canBorrow { get; set; }
    }
}
