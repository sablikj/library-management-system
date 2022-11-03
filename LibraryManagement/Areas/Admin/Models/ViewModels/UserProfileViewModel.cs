using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using LibraryManagement.Models.Entity;

namespace LibraryManagement.Areas.Admin.Models.ViewModels
{
    public class UserProfileViewModel
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

        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string Email { get; set; }

        public IList<Book> RentedBooks { get; set; }

        [Required]
        public bool Approved { get; set; }

        public bool Banned { get; set; }

        public IList<Loan> Loans { get; set; }
        public IList<Book> Books { get; set; }
    }
}
