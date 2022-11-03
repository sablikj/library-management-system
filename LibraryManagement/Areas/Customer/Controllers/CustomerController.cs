
using LibraryManagement.Areas.Admin.Controllers;
using LibraryManagement.Areas.Admin.Models.ViewModels;
using LibraryManagement.Areas.Customer.Models.ViewModels;
using LibraryManagement.Models.Entity;
using LibraryManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Security.Claims;

namespace LibraryManagement.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = "Customer")]
    public class CustomerController : Controller
    {
        protected DatabaseService dbService;
        IWebHostEnvironment env;
        
        public CustomerController(DatabaseService dbService, IWebHostEnvironment env)
        {
            this.dbService = dbService;   
            this.env = env;
        }
        
        public async Task<IActionResult> Index()
        {            
            Guid customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (customerId == Guid.Empty)
            {
                return NotFound();
            }
            var userFilter = Builders<User>.Filter.Eq(user => user.Id, customerId);                        
            
            // Get current user            
            User customer = await dbService.usersCollection.Find(userFilter).FirstOrDefaultAsync();
            if (customer == null)
            {
                return NotFound();
            }

            // Retriving Books from db
            IList<Book> rentedBooks = new List<Book>();
            foreach (var item in customer.RentedBooks)
            {
                var bookFilter = Builders<Book>.Filter.Eq(book => book.Id, item);
                rentedBooks.Add(await dbService.bookCollection.Find(bookFilter).FirstOrDefaultAsync());
            }            

            // Getting loan history
            var loanFilter = Builders<Loan>.Filter.Eq(loan => loan.UserId, customer.Id);
            IList<Loan> loans = await dbService.loanCollection.Find(loanFilter).ToListAsync();

            IList<Book> books = new List<Book>();
            // Getting books from past loans
            foreach (var lo in loans)
            {
                foreach (var item in lo.LoanItems)
                {
                    var bookFilter = Builders<Book>.Filter.Eq(book => book.Id, item);
                    books.Add(await dbService.bookCollection.Find(bookFilter).FirstOrDefaultAsync());
                }
            }

            UserProfileViewModel customerVM = new UserProfileViewModel()
            {
                Id = customer.Id,
                Name = customer.Name,
                Surname = customer.Surname,
                SSN = customer.SSN,
                City = customer.City,
                Street = customer.Street,
                HouseNumber = customer.HouseNumber,
                ZipCode = customer.ZipCode,
                Username = customer.Username,
                Email = customer.Email,
                RentedBooks = rentedBooks, // Here, rented books are instances of Book class, NOT Guid
                Approved = customer.Approved,
                Banned = customer.Banned,
                Loans = loans,
                Books = books
            };       
            return View(customerVM);
        }

        // GET: Customer/Customer/Edit/id
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var filter = Builders<User>.Filter.Eq(user => user.Id, id);
            var user = await dbService.usersCollection.Find(filter).FirstOrDefaultAsync();
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Customer/Customer/Edit/id
        // Customer can't update Username and password
        [HttpPost]
        public async Task<IActionResult> Edit(Guid? id, User userEdit)
        {
            if (id == null || userEdit == null)
            {                
                return NotFound();
            }
            if (ModelState.IsValid)
            {              
                var filter = Builders<User>.Filter.Eq(u => u.Id, userEdit.Id);                
                var update = Builders<User>.Update
                        .Set(c => c.Name, userEdit.Name)
                        .Set(c => c.Surname, userEdit.Surname)
                        .Set(c => c.SSN, userEdit.SSN)
                        .Set(c => c.City, userEdit.City)
                        .Set(c => c.Street, userEdit.Street)
                        .Set(c => c.HouseNumber, userEdit.HouseNumber)
                        .Set(c => c.ZipCode, userEdit.ZipCode)
                        .Set(c => c.Email, userEdit.Email)
                        .Set(c => c.Approved, false); // User has to be approved again
                
                var result = await dbService.usersCollection.UpdateOneAsync(filter, update);               
                if (result.IsAcknowledged)
                {
                    return RedirectToAction(nameof(CustomerController.Index), nameof(CustomerController).Replace("Controller", ""), new { area = "Customer" });
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
                ViewBag.ErrorMessage = error.ErrorMessage;
            }
            return View(userEdit);
        }

        public async Task<IActionResult> ReturnBook(Guid userId, Guid bookId)
        {
            if (userId == Guid.Empty && bookId == Guid.Empty)
            {
                return NotFound();
            }

            // Remove book from user 
            var userFilter = Builders<User>.Filter.Eq(user => user.Id, userId);
            User user = await dbService.usersCollection.Find(userFilter).FirstOrDefaultAsync();
            if (user == null)
            {
                return NotFound();
            }

            user.RentedBooks.Remove(bookId);
            var update = Builders<User>.Update
                                .Set(u => u.RentedBooks, user.RentedBooks);

            var userResult = await dbService.usersCollection.UpdateOneAsync(userFilter, update);
            if (!userResult.IsAcknowledged)
            {
                // TODO: Add error view
                return NotFound();
            }

            // Edit book quantity
            var bookFilter = Builders<Book>.Filter.Eq(book => book.Id, bookId);
            Book book = await dbService.bookCollection.Find(bookFilter).FirstOrDefaultAsync();
            if (book == null)
            {
                return NotFound();
            }

            book.Quantity += 1;
            var bookUpdate = Builders<Book>.Update
                                .Set(b => b.Quantity, book.Quantity);
            var bookResult = await dbService.bookCollection.UpdateOneAsync(bookFilter, bookUpdate);
            if (!bookResult.IsAcknowledged)
            {
                return NotFound();
            }
            if (userResult.ModifiedCount == 1 && bookResult.ModifiedCount == 1)
            {
                return RedirectToAction(nameof(CustomerController.Index), nameof(CustomerController).Replace("Controller", ""), new { area = "Customer", id = user.Id });
            }
            else
            {
                return ViewBag.ErrorMessage = "Book remove error!";
            }
        }
    }
}
