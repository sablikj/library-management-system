
using LibraryManagement.Models.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagement.Models.Entity
{
    public class CarouselItem
    {
        [Key]
        [Required]
        public int ID { get; set; }

        [ContentType("image")]
        [FileSize(15_000_000)] // 5 MB max file size
        [NotMapped]
        public IFormFile Image { get; set; }

        [Required]
        [StringLength(255)]
        public string ImageSource { get; set; }

        [StringLength(50)]
        public string ImageAlt { get; set; }
    }
}
