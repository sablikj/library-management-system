using LibraryManagement.Areas.Admin.Models.ViewModels;
using LibraryManagement.Models.Entity;
using LibraryManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using MongoDB.Driver;
using NuGet.Packaging;
using System.Data;

namespace LibraryManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Librarian")]
    public class LoanController : Controller
    {
        protected DatabaseService dbService;

        public LoanController(DatabaseService dbService)
        {
            this.dbService = dbService;
        }
        public IActionResult Index()
        {
            CompleteViewModel completeVM = new CompleteViewModel()
            {
                Loans = dbService.loanCollection.AsQueryable<Loan>().ToList(),
                Books = dbService.bookCollection.AsQueryable<Book>().ToList(),
                Users = dbService.usersCollection.AsQueryable<User>().ToList()
            };
            return View(completeVM);
        }
        public IActionResult Create()
        {
            LoanViewModel loanVM = new LoanViewModel()
            {
                Books = dbService.bookCollection.AsQueryable<Book>().ToList(),
                Users = dbService.usersCollection.AsQueryable<User>().ToList()
            };
            return View(loanVM);
        }

        [HttpPost]
        public async Task<IActionResult> Create(LoanViewModel loanVM)
        {
            if (loanVM != null && loanVM.LoanItems.Count > 0)
            {
                string s = "";
                Loan loan = new Loan()
                {
                    Id = loanVM.Id,
                    UserId = loanVM.UserId,
                    CreatedOn = DateTime.Now,
                    Valid = true,
                    LoanItems = loanVM.LoanItems,
                    BookNames = string.Empty
                };

                for (int i = 0; i < loanVM.Books.Count; i++)
                {
                    if(i != loanVM.Books.Count - 1)
                    {
                        loan.BookNames += loanVM.Books[i].Name + ", ";
                    }
                    else
                    {
                        loan.BookNames += loanVM.Books[i].Name;
                    }                    
                }

                // Book quantity edit
                foreach(var item in loan.LoanItems)
                {
                    var filter = Builders<Book>.Filter.Eq(b => b.Id, item);
                    Book book = await dbService.bookCollection.Find(filter).FirstOrDefaultAsync();
                    
                    var update = Builders<Book>.Update                            
                            .Set(b => b.Available, book.Available - 1);

                    var result = await dbService.bookCollection.UpdateOneAsync(filter, update);
                    if (result.IsAcknowledged)
                    {
                        continue;
                    }
                    else
                    {
                        // TODO: Add error view
                        return NotFound();
                    }
                }

                // Adding books to User account               
                var userFilter = Builders<User>.Filter.Eq(u => u.Id, loan.UserId);
                User user = await dbService.usersCollection.Find(userFilter).FirstOrDefaultAsync();

                IList<Guid> rentedBooks = user.RentedBooks;
                rentedBooks.AddRange(loan.LoanItems);

                var userUpdate = Builders<User>.Update
                            .Set(u => u.RentedBooks, rentedBooks);

                var userResult = await dbService.usersCollection.UpdateOneAsync(userFilter, userUpdate);
                if (!userResult.IsAcknowledged)
                {
                    // TODO: Add error view
                    return NotFound();
                }                

                // Add loan to DB
                await dbService.loanCollection.InsertOneAsync(loan);
                ViewBag.Message = "Loan created successfully.";

                return RedirectToAction(nameof(LoanController.Index), nameof(LoanController).Replace("Controller", ""), new { area = "Admin" });
            }
            return NotFound();
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var filter = Builders<Loan>.Filter.Eq(loan => loan.Id, id);
            var loan = await dbService.loanCollection.Find(filter).FirstOrDefaultAsync();
            if (loan == null)
            {
                return NotFound();
            }

            LoanViewModel loanVM = new LoanViewModel()
            {
                Id = loan.Id,                
                UserId = loan.UserId,
                CreatedOn = loan.CreatedOn, 
                Valid = loan.Valid,
                LoanItems = loan.LoanItems,
                Books = dbService.bookCollection.AsQueryable<Book>().ToList(),
                Users = dbService.usersCollection.AsQueryable<User>().ToList()
            };

            return View(loanVM);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Guid? id, LoanViewModel loanVM)
        {
            if (id == null && loanVM == null)
            {
                return NotFound();
            }
            
            Loan loanEdit = new Loan()
            {
                Id = loanVM.Id,
                UserId = loanVM.UserId,
                LoanItems = loanVM.LoanItems                
            };
            
            if (loanEdit == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var filter = Builders<Loan>.Filter.Eq(loan => loan.Id, loanEdit.Id);
                var update = Builders<Loan>.Update
                        .Set(l => l.UserId, loanEdit.UserId)
                        .Set(l => l.LoanItems, loanEdit.LoanItems);                        

                var result = await dbService.loanCollection.UpdateOneAsync(filter, update);
                if (result.IsAcknowledged)
                {
                    return RedirectToAction(nameof(LoanController.Index), nameof(LoanController).Replace("Controller", ""), new { area = "Admin" });
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
            return NotFound();
        }

        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Book quantity edit
            var filter = Builders<Loan>.Filter.Eq(loan => loan.Id, id);
            Loan loan = await dbService.loanCollection.Find(filter).FirstOrDefaultAsync();
            if (loan == null)
            {
                return NotFound();
            }

            foreach (var item in loan.LoanItems)
            {
                var bookFilter = Builders<Book>.Filter.Eq(book => book.Id, item);
                Book book = await dbService.bookCollection.Find(bookFilter).FirstOrDefaultAsync();

                var update = Builders<Book>.Update
                            .Set(b => b.Available, book.Available + 1);

                var bookResult = await dbService.bookCollection.UpdateOneAsync(bookFilter, update);
                if (bookResult.IsAcknowledged)
                {
                    continue;
                }
                else
                {
                    // TODO: Add error view
                    return NotFound();
                }
            }

            // Removing books from User account               
            var userFilter = Builders<User>.Filter.Eq(u => u.Id, loan.UserId);
            User user = await dbService.usersCollection.Find(userFilter).FirstOrDefaultAsync();

            IList<Guid> updatedBooks = user.RentedBooks;
            foreach (var item in loan.LoanItems)
            {
                updatedBooks.Remove(item);
            }            

            var userUpdate = Builders<User>.Update
                        .Set(u => u.RentedBooks, updatedBooks);

            var userResult = await dbService.usersCollection.UpdateOneAsync(userFilter, userUpdate);
            if (!userResult.IsAcknowledged)
            {
                // TODO: Add error view
                return NotFound();
            }

            // Add loan to DB
            var result = await dbService.loanCollection.DeleteOneAsync(filter);
            if (result.DeletedCount == 1)
            {
                return RedirectToAction(nameof(LoanController.Index), nameof(LoanController).Replace("Controller", ""), new { area = "Admin" });
            }
            else
            {
                return ViewBag.ErrorMessage = "Loan Delete Error!";
            }
        } 
        
        public async Task<IActionResult> Details(Guid id)
        {
            if(id == Guid.Empty)
            {
                return NotFound();
            }

            var loanFilter = Builders<Loan>.Filter.Eq(loan => loan.Id, id);
            var loan = await dbService.loanCollection.Find(loanFilter).FirstOrDefaultAsync();
            if (loan == null)
            {
                return NotFound();
            }            

            LoanViewModel loanVM = new LoanViewModel()
            {
                Id = loan.Id,
                UserId = loan.UserId,
                CreatedOn = loan.CreatedOn,
                LoanItems = loan.LoanItems,
                Books = new List<Book>(),
                Users = new List<User>()
            };

            var userFilter = Builders<User>.Filter.Eq(user => user.Id, loan.UserId);
            var user = await dbService.usersCollection.Find(userFilter).FirstOrDefaultAsync();

            if(user == null)
            {
                return NotFound();
            }

            loanVM.Users.Add(user);

            // Books
            foreach (var bookId in loan.LoanItems)
            {
                var bookFilter = Builders<Book>.Filter.Eq(book => book.Id, bookId);
                loanVM.Books.Add(await dbService.bookCollection.Find(bookFilter).FirstOrDefaultAsync());
            }

            return View(loanVM);
        }
    }
}
