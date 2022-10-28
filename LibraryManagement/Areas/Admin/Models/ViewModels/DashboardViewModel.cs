using LibraryManagement.Models.Entity;

namespace LibraryManagement.Areas.Admin.Models.ViewModels
{
    public class DashboardViewModel
    {
        public IList<User> Users { get; set; }
        public IList<Loan> Loans { get; set; }
        public IList<Book> Books { get; set; }
    }
}
