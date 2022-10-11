using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LibraryManagement.Models
{
    public class Book
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("Name")]
        public string BookName { get; set; } = null!;
        public string Author { get; set; } = null!;
        public int Pages { get; set; }
        public int Year { get; set; }
        public string ISBN { get; set; } = null!;
        public byte[] Image { get; set; } = null!;
        public int Quantity { get; set; }
    }
}
