using LibraryManagement.Models.Entity;

namespace LibraryManagement.Areas.Customer.Models.ViewModels
{
    public class CustomerViewModel
    {
        public User Customer { get; set; }
        public IList<Loan> Loans { get; set; }

        public IList<Book> RentedBooks { get; set; }      

        public IList<Book> Books { get; set; }
    }
}
