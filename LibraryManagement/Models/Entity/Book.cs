using LibraryManagement.Models.Validation;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models.Entity
{
    public class Book
    {
        [BsonId]
        [Required]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid Id { get; set; }

        [Required]
        [StringLength(120)]
        public string Name { get; set; }

        [Required]
        [StringLength(120)]
        public string Author { get; set; } 

        [Required]
        [StringLength(2999)]
        public string Description { get; set; }

        [Required]
        public int Pages { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        [StringLength(120)]
        public string ISBN { get; set; }

        //[ContentType("image")]
        [FileSize(15_000_000)] // 15 MB max file size
        [BsonIgnore]
        public IFormFile Image { get; set; }

        [ValidateNever]
        public byte[] ImageContent { get; set; }

        [Required]
        [StringLength(50)]
        public string ImageAlt { get; set; }

        [Required]
        public int Quantity { get; set; }
        public int Available { get; set; }
    }
}
