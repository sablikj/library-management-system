using LibraryManagement.Models.Validation;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Areas.Admin.Models.ViewModels
{
    public class imageEditViewModel
    {
        [BsonId]
        [Required]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid Id { get; set; }

        [ContentType("image")]
        [FileSize(15_000_000)] // 15 MB max file size
        [BsonIgnore]
        public IFormFile Image { get; set; }

        [ValidateNever]
        public byte[] ImageContent { get; set; }

        [Required]
        [StringLength(50)]
        public string ImageAlt { get; set; }

    }
}
