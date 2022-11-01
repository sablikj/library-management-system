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
                Loan loan = new Loan()
                {
                    Id = loanVM.Id,
                    UserId = loanVM.UserId,
                    CreatedOn = DateTime.Now,
                    Valid = true,
                    LoanItems = loanVM.LoanItems
                };

                // Book quantity edit
                foreach(var item in loan.LoanItems)
                {
                    var filter = Builders<Book>.Filter.Eq(b => b.Id, item);
                    Book book = await dbService.bookCollection.Find(filter).FirstOrDefaultAsync();
                    
                    var update = Builders<Book>.Update                            
                            .Set(b => b.Quantity, book.Quantity - 1);

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
                            .Set(b => b.Quantity, book.Quantity + 1);

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
    }
}
