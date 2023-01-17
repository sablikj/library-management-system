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

        [BsonRepresentation(BsonType.DateTime)]
        public DateTime CreatedOn { get; set; }
               
        [Required]        
        public Guid UserId { get; set; }

        public bool Valid { get; set; }
                
        [Required]
        public IList<Guid> LoanItems { get; set; }

        public string? BookNames { get; set; }
    }
}
