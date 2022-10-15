namespace LibraryManagement.Models
{
    public class DatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string BooksCollectionName { get; set; } = null!;
    }
}
