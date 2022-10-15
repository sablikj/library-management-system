using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models.Entity
{
    public class BookLoan
    {
        [Key]
        [Required]
        public int ID { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime DateTimeCreated { get; protected set; }

        [StringLength(25)]
        [Required]
        public string OrderNumber { get; set; }

        [Required]
        public double TotalPrice { get; set; }

        //[ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public User User { get; set; }

        //[ForeignKey(nameof(OrderStatus))]
        //public int OrderStatusId { get; set; }
        //public OrderStatus OrderStatus { get; set; }

        public IList<LoanItem> LoanItems { get; set; }
    }
}
