using LibraryManagement.Models.Entity;

namespace LibraryManagement.Areas.Admin.Models.ViewModels
{
    public class CompleteViewModel
    {
        public IList<User> Users { get; set; }
        public IList<Loan> Loans { get; set; }
        public IList<Book> Books { get; set; }
    }
}
