using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace LibraryManagement.Models.Entity
{
    public class Loan
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
        public IList<Book> LoanItems { get; set; }
    }
}
