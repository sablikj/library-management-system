using LibraryManagement.Areas.Admin.Models.ViewModels;
using LibraryManagement.Models.Entity;
using LibraryManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Data;

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
        
        public IActionResult Index()
        {
            return View(dbService.bookCollection.AsQueryable<Book>().ToList());
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
                Quantity = bookEditVM.Quantity
            };

            if (bookEdit == null)
            {
                return NotFound();
            }            

            if (ModelState.IsValid)
            {                
                var filter = Builders<Book>.Filter.Eq(book => book.Id, bookEdit.Id);
                var update = Builders<Book>.Update
                        .Set(b => b.Name, bookEdit.Name)
                        .Set(b => b.Author, bookEdit.Author)
                        .Set(b => b.Description, bookEdit.Description)
                        .Set(b => b.Pages, bookEdit.Pages)
                        .Set(b => b.Year, bookEdit.Year)
                        .Set(b => b.ISBN, bookEdit.ISBN)                        
                        .Set(b => b.Quantity, bookEdit.Quantity);

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
    }
}
