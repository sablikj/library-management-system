using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using MongoDbGenericRepository.Attributes;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace LibraryManagement.Models.Entity
{
    [CollectionName("Users")]
    [BsonIgnoreExtraElements]
    public class User
    {
        [BsonId]
        [Required]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }

        [Required]
        public string Surname { get; set; }

        [Required]
        public int SSN { get; set; }

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
        
        public string? Password { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string Email { get; set; }

        [ValidateNever]
        public IList<Guid> RentedBooks { get; set; }

        [Required]
        public bool Approved { get; set; }

        public bool Banned { get; set; }

        [BsonRepresentation(BsonType.DateTime)]
        public DateTime? BannedDate { get; set; }
    }
}
