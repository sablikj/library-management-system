using LibraryManagement.Models.Validation;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Areas.Admin.Models.ViewModels
{
    public class bookEditViewModel
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

        [Required]
        public int Quantity { get; set; }
        public int Available { get; set; }
    }
}
