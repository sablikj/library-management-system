using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using MongoDbGenericRepository.Attributes;

namespace LibraryManagement.Models.Entity
{
    [CollectionName("Users")]
    [BsonIgnoreExtraElements]
    public class User
    {
        [BsonId]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }

        [Required]
        public string Surname { get; set; }

        [Required]
        public int SNN { get; set; }

        [Required]
        public string City { get; set; }
        
        [Required]
        public string Street { get; set; }

        [Required]
        public int HouseNumber { get; set; }

        [Required]
        public int ZipCode { get; set; }

        [Required]
        [BsonElement("UserName")]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string Email { get; set; }

        public string[]? RentedBooks { get; set; }

        [Required]
        public bool Approved { get; set; }

        public bool Banned { get; set; }
    }
}
