namespace LibraryManagement.Models.Entity
{
    public class LoanItem
    {
        public Guid Id { get; set; }
        public Guid BookId { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
