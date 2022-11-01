using LibraryManagement.Models;
using LibraryManagement.Models.Entity;
using LibraryManagement.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
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
    }
}