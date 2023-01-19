using LibraryManagement.Areas.Admin.Models.ViewModels;
using LibraryManagement.Models;
using LibraryManagement.Models.Entity;
using LibraryManagement.Models.ViewModels;
using LibraryManagement.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDbGenericRepository.Utils;
using System.Diagnostics;
using System.Globalization;
using System.Security.Claims;

namespace LibraryManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;
        protected DatabaseService dbService;

        public HomeController(ILogger<HomeController> logger, DatabaseService dbService)
        {
            this.logger = logger;
            this.dbService = dbService;
        }

        public IActionResult Index()
        {
            IndexViewModel indexVM = new IndexViewModel();
            // Only available books
            FilterDefinition<Book> filter = Builders<Book>.Filter.Gt(b => b.Available, 0);

            // Popular books - least available
            SortDefinition<Book> sortPopular = Builders<Book>.Sort.Ascending(b => b.Available);
            indexVM.PopularBooks = dbService.bookCollection.Find(filter).Sort(sortPopular).Limit(3).ToList();

            // Random book            
            Random rnd = new Random();
            indexVM.SelectedBooks = dbService.bookCollection.Find(filter).ToList().OrderBy(x => rnd.Next()).Take(8).ToList();             

            //return View(dbService.bookCollection.AsQueryable<Book>().ToList());
            return View(indexVM);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Details(Guid? id, string? username)
        {
            if (id == null)
            {
                return NotFound();
            }           

            var filter = Builders<Book>.Filter.Eq(book => book.Id, id);
            var book = await dbService.bookCollection.Find(filter).FirstOrDefaultAsync();
            if (book == null)
            {
                return NotFound();
            }

            DetailsViewModel bookDetail = new DetailsViewModel();
            bookDetail.book = book;
            bookDetail.canBorrow = false;

            // Check if book can be borrowed
            if(username != "noUser")
            {
                var userFilter = Builders<User>.Filter.Eq(u => u.Username, username);
                User user = await dbService.usersCollection.Find(userFilter).FirstAsync();

                if (user == null)
                {
                    return NotFound();
                }

                if (user.RentedBooks == null)
                {
                    bookDetail.canBorrow = true;
                }
                else if (user.RentedBooks.Contains((Guid)id))
                {
                    bookDetail.canBorrow = false;
                }
                else
                {
                    bookDetail.canBorrow = true;
                }
            }           

            return View(bookDetail);
        }

        public IActionResult Books(bookIndexViewModel bookIndexVM)
        {
            bookIndexVM.Books = dbService.bookCollection.AsQueryable<Book>().ToList();
            return View(bookIndexVM);
        }

        public async Task<IActionResult> Search(bookIndexViewModel bookIndexVM)
        {
            if (bookIndexVM != null)
            {
                var builder = Builders<Book>.Filter;
                FilterDefinition<Book> filterName = builder.Empty;
                FilterDefinition<Book> filterYear = builder.Empty;
                FilterDefinition<Book> filterAuthor = builder.Empty;

                if (bookIndexVM.SearchBook != null)
                {
                    filterName = builder.Regex(b => b.Name, new BsonRegularExpression(bookIndexVM.SearchBook.Pascalize()));
                }
                if (bookIndexVM.SearchYear != 0 && bookIndexVM.SearchYear != null)
                {
                    filterYear = builder.Eq(b => b.Year, bookIndexVM.SearchYear);
                }
                if (bookIndexVM.SearchAuthor != null)
                {
                    filterAuthor = builder.Regex(b => b.Author, new BsonRegularExpression(bookIndexVM.SearchAuthor.Pascalize()));
                }

                var filter = builder.And(new[] { filterName, filterYear, filterAuthor });
                List<Book> books = new List<Book>();
                books = await dbService.bookCollection.Find(filter).ToListAsync();

                if (books != null)
                {
                    bookIndexVM.Books = books;
                    return View("Books", bookIndexVM);
                }
            }            

            return NotFound();
        }

        public async Task<IActionResult> Sort(bookIndexViewModel books, string filter)
        {
            if(books != null)
            {
                bookIndexViewModel bookIndexVM = new bookIndexViewModel();
                FilterDefinition<Book> sortFilter = Builders<Book>.Filter.Empty;

                if (filter == "Name")
                {                   
                    SortDefinition<Book> sort = Builders<Book>.Sort.Ascending(b => b.Name);
                    bookIndexVM.Books = dbService.bookCollection.Find(sortFilter).Sort(sort).ToList();                   
                }
                else if(filter == "Author")
                {
                    SortDefinition<Book> sort = Builders<Book>.Sort.Ascending(b => b.Author);
                    bookIndexVM.Books = dbService.bookCollection.Find(sortFilter).Sort(sort).ToList();
                }
                else
                {
                    SortDefinition<Book> sort = Builders<Book>.Sort.Ascending(b => b.Year);
                    bookIndexVM.Books = dbService.bookCollection.Find(sortFilter).Sort(sort).ToList();
                }

                return View("Books", bookIndexVM);
            }

            return NotFound();
        }
    }
}