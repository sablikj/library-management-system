using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models.Entity
{
    public class Book
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [Required]
        public string? Id { get; set; }

        [Required]
        [StringLength(120)]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(120)]
        public string Author { get; set; } = null!;

        [Required]
        [StringLength(2999)]
        public string Description { get; set; } = null!;

        [Required]
        public int Pages { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        [StringLength(120)]
        public string ISBN { get; set; } = null!;

        [Required]
        public byte[] ImageSource { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string ImageAlt { get; set; } = null!;

        [Required]
        public int Quantity { get; set; }
    }
}
