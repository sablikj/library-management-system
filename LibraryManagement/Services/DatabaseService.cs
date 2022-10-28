using LibraryManagement.Models;
using LibraryManagement.Models.Entity;
using Microsoft.Extensions.Options;
using MongoDB.Driver;


namespace LibraryManagement.Services
{
    public class DatabaseService
    {
        //private readonly IMongoDatabase _database;
        //private readonly IMongoCollection<Book> _booksCollection;

        public IMongoCollection<Book> bookCollection { get; set; }
        public IMongoCollection<User> usersCollection { get; set; }
        public IMongoCollection<Loan> loanCollection { get; set; }


        public DatabaseService(IOptions<DatabaseSettings> databaseSettings)
        {
            var mongoClient = new MongoClient(
                databaseSettings.Value.ConnectionString);

            var database = mongoClient.GetDatabase(
                databaseSettings.Value.Name);

            bookCollection = database.GetCollection<Book>("Book");
            usersCollection = database.GetCollection<User>("Users");
            loanCollection = database.GetCollection<Loan>("Loan");            
        }

        // CRUD operations
        public async Task<List<Book>> GetAsync() =>
            await bookCollection.Find(_ => true).ToListAsync();

        public async Task<Book?> GetAsync(string id) =>
            await bookCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Book newBook) =>
            await bookCollection.InsertOneAsync(newBook);

        public async Task UpdateAsync(string id, Book updatedBook) =>
            await bookCollection.ReplaceOneAsync(x => x.Id == id, updatedBook);

        public async Task RemoveAsync(string id) =>
            await bookCollection.DeleteOneAsync(x => x.Id == id);
    }
}
