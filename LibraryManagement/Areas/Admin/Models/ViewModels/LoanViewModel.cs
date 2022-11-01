using LibraryManagement.Models.Entity;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Areas.Admin.Models.ViewModels
{
    public class LoanViewModel
    {
        [BsonId]
        [Required]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid Id { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime Date { get; protected set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public IList<Guid> LoanItems { get; set; }
        public IList<Book> Books { get; set; }
        public IList<User> Users { get; set; }
    }
}
