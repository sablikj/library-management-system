using LibraryManagement.Models.Entity;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace LibraryManagement.Areas.Admin.Models.ViewModels
{
    public class LoanViewModel
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

        [ValidateNever]
        public IList<Book> Books { get; set; }
        [ValidateNever]
        public IList<User> Users { get; set; }
    }
}
