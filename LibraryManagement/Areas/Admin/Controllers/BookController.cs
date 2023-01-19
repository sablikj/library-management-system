using LibraryManagement.Areas.Admin.Models.ViewModels;
using LibraryManagement.Models.Entity;
using LibraryManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDbGenericRepository.Utils;
using System.Data;
using System.Linq;
using static System.Formats.Asn1.AsnWriter;

namespace LibraryManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Librarian")]
    public class BookController : Controller
    {
        protected DatabaseService dbService;

        public BookController(DatabaseService dbService)
        {
            this.dbService = dbService;
        }
        
        public IActionResult Index(bookIndexViewModel bookIndexVM)
        {            
            bookIndexVM.Books = dbService.bookCollection.AsQueryable<Book>().ToList();
            return View(bookIndexVM);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Book book, IFormFile Image)
        {
            if(book != null)
            {
                book.Available = book.Quantity;

                if(Image != null && Image.Length > 0)
                {
                    using (var target = new MemoryStream())
                    {
                        Image.CopyTo(target);
                        book.ImageContent = target.ToArray();
                    }
                }
                await dbService.CreateAsync(book);
                ViewBag.Message = "Book created successfully.";

                return RedirectToAction(nameof(BookController.Index), nameof(BookController).Replace("Controller", ""), new { area = "Admin" });
            }
            return NotFound();
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

        public async Task<IActionResult> dataEdit(Guid? id)
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

            bookEditViewModel bookVM = new bookEditViewModel
            {
                Id = book.Id,
                Name = book.Name,
                Author = book.Author,
                Description = book.Description,
                Pages = book.Pages,
                Year = book.Year,
                ISBN = book.ISBN,
                Available = book.Available,
                Quantity = book.Quantity
            };

            return View(bookVM);
        }

        [HttpPost]
        public async Task<IActionResult> dataEdit(Guid? id, bookEditViewModel bookEditVM)
        {
            if (id == null || bookEditVM == null)
            {
                return NotFound();
            } 

            Book bookEdit = new Book()
            {
                Id = bookEditVM.Id,
                Name = bookEditVM.Name,
                Author = bookEditVM.Author,
                Description = bookEditVM.Description,
                Pages = bookEditVM.Pages,
                Year = bookEditVM.Year,
                ISBN = bookEditVM.ISBN,       
                Available = bookEditVM.Available,
                Quantity = bookEditVM.Quantity                
            };

            if (bookEdit == null)
            {
                return NotFound();
            }            

            if (ModelState.IsValid)
            {                
                var filter = Builders<Book>.Filter.Eq(book => book.Id, bookEdit.Id);
                // get data of original book
                Book book = await dbService.bookCollection.Find(filter).FirstOrDefaultAsync();
                // update available books
                
                // Quantity decrease
                if(book.Quantity > bookEdit.Quantity)
                {
                    book.Available = book.Available - (book.Quantity - bookEdit.Quantity);
                }
                // Quantity increase
                else if(book.Quantity < bookEdit.Quantity)
                {
                    book.Available = book.Available + (bookEdit.Quantity - book.Quantity);
                }
                
                var update = Builders<Book>.Update
                        .Set(b => b.Name, bookEdit.Name)
                        .Set(b => b.Author, bookEdit.Author)
                        .Set(b => b.Description, bookEdit.Description)
                        .Set(b => b.Pages, bookEdit.Pages)
                        .Set(b => b.Year, bookEdit.Year)
                        .Set(b => b.ISBN, bookEdit.ISBN)
                        .Set(b => b.Quantity, bookEdit.Quantity)
                        .Set(b => b.Available, book.Available);

                var result = await dbService.bookCollection.UpdateOneAsync(filter, update);
                if (result.IsAcknowledged)
                {
                    return RedirectToAction(nameof(BookController.Index), nameof(BookController).Replace("Controller", ""), new { area = "Admin" });
                }
                else
                {
                    // TODO: Add error view
                    return NotFound();
                }
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                Console.WriteLine(error.ErrorMessage);
                ViewBag.ErrorMessage = error.ErrorMessage;
            }
            return View(bookEdit);
        }

        public async Task<IActionResult> imageEdit(Guid? id)
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

            imageEditViewModel imageVM = new imageEditViewModel
            {
                Id = book.Id,
                ImageContent = book.ImageContent,
                ImageAlt = book.ImageAlt
            };

            return View(imageVM);
        }

        [HttpPost]
        public async Task<IActionResult> imageEdit(Guid? id, IFormFile Image, imageEditViewModel imageEdit)
        {
            if (id == null || imageEdit == null)
            {
                return NotFound();
            }
            if (Image != null && Image.Length > 0)
            {
                using (var target = new MemoryStream())
                {
                    Image.CopyTo(target);
                    imageEdit.ImageContent = target.ToArray();
                }
            }

            if (ModelState.IsValid)
            {

                var filter = Builders<Book>.Filter.Eq(book => book.Id, imageEdit.Id);
                var update = Builders<Book>.Update
                        .Set(b => b.ImageContent, imageEdit.ImageContent)
                        .Set(b => b.ImageAlt, imageEdit.ImageAlt);
                        

                var result = await dbService.bookCollection.UpdateOneAsync(filter, update);
                if (result.IsAcknowledged)
                {
                    return RedirectToAction(nameof(BookController.Index), nameof(BookController).Replace("Controller", ""), new { area = "Admin" });
                }
                else
                {
                    // TODO: Add error view
                    return NotFound();
                }
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                Console.WriteLine(error.ErrorMessage);
                ViewBag.ErrorMessage = error.ErrorMessage;
            }
            return View(imageEdit);
        }

        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filter = Builders<Book>.Filter.Eq(book => book.Id, id);
            var result = await dbService.bookCollection.DeleteOneAsync(filter);

            if (result.DeletedCount == 1)
            {
                return RedirectToAction(nameof(BookController.Index), nameof(BookController).Replace("Controller", ""), new { area = "Admin" });
            }
            else
            {
                return ViewBag.ErrorMessage = "Book Delete Error!";
            }
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
                    return View("Index", bookIndexVM);
                }
            }        

            return NotFound();
        }

        public async Task<IActionResult> Sort(bookIndexViewModel books, string filter)
        {
            if (books != null)
            {
                bookIndexViewModel bookIndexVM = new bookIndexViewModel();
                FilterDefinition<Book> sortFilter = Builders<Book>.Filter.Empty;

                if (filter == "Name")
                {
                    SortDefinition<Book> sort = Builders<Book>.Sort.Ascending(b => b.Name);
                    bookIndexVM.Books = dbService.bookCollection.Find(sortFilter).Sort(sort).ToList();
                }
                else if (filter == "Author")
                {
                    SortDefinition<Book> sort = Builders<Book>.Sort.Ascending(b => b.Author);
                    bookIndexVM.Books = dbService.bookCollection.Find(sortFilter).Sort(sort).ToList();
                }
                else
                {
                    SortDefinition<Book> sort = Builders<Book>.Sort.Ascending(b => b.Year);
                    bookIndexVM.Books = dbService.bookCollection.Find(sortFilter).Sort(sort).ToList();
                }

                return View("Index", bookIndexVM);
            }

            return NotFound();
        }
    }
}
