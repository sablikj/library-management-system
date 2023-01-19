using AspNetCore.Identity.MongoDbCore.Models;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoDbGenericRepository.Attributes;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models.Identity
{
    [CollectionName("Users")]
    public class ApplicationUser : MongoIdentityUser<Guid>
    {
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

        public IList<Guid> RentedBooks { get; set; }

        [Required]
        public bool Approved { get; set; }

        public bool Banned { get; set; }

        [BsonRepresentation(BsonType.DateTime)]
        public DateTime? BannedDate { get; set; }
    }
}
