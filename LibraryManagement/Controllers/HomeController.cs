using LibraryManagement.Areas.Admin.Models.ViewModels;
using LibraryManagement.Models;
using LibraryManagement.Models.Entity;
using LibraryManagement.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDbGenericRepository.Utils;
using System.Diagnostics;
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
            return View(dbService.bookCollection.AsQueryable<Book>().ToList());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Details(Guid? id)
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

            return View(book);
        }

        public IActionResult Books(bookIndexViewModel bookIndexVM)
        {
            bookIndexVM.Books = dbService.bookCollection.AsQueryable<Book>().ToList();
            return View(bookIndexVM);
        }

        // TODO: Rework SEARCH function (to loop)
        public async Task<IActionResult> Search(bookIndexViewModel bookIndexVM)
        {
            if (bookIndexVM == null)
            {
                return NotFound();
            }

            var builder = Builders<Book>.Filter;
            FilterDefinition<Book> filterName = builder.Empty;
            FilterDefinition<Book> filterYear = builder.Empty;
            FilterDefinition<Book> filterAuthor = builder.Empty;

            if (bookIndexVM.SearchBook != null)
            {
                filterName = builder.Regex(b => b.Name, new BsonRegularExpression(bookIndexVM.SearchBook.Pascalize()));
            }
            else if (bookIndexVM.SearchYear != 0 && bookIndexVM.SearchYear != null)
            {
                filterYear = builder.Eq(b => b.Year, bookIndexVM.SearchYear);
            }
            else if (bookIndexVM.SearchAuthor != null)
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

            return NotFound();
        }
    }
}