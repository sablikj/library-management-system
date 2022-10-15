using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagement.Models.Entity
{
    public class LoanItem
    {
        [Key]
        [Required]
        public int ID { get; set; }

        //[ForeignKey(nameof(BookLoan))]
        public int OrderID { get; set; }

        //[ForeignKey(nameof(Book))]
        public int ProductID { get; set; }

        //public int Amount { get; set; }
        //public double Price { get; set; }

        public BookLoan BookLoan { get; set; }
        public Book Book { get; set; }
    }
}
