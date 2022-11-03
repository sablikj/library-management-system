﻿using LibraryManagement.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LibraryManagement.Models.Entity;
using LibraryManagement.Services;
using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using LibraryManagement.Areas.Admin.Models.ViewModels;

namespace LibraryManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Librarian")]
    public class UserController : Controller
    {
        private UserManager<ApplicationUser> userManager;
        private RoleManager<ApplicationRole> roleManager;
        private readonly DatabaseService dbService;
        public UserController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, DatabaseService dbService)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.dbService = dbService;
        }

        public IActionResult Index()
        {
            return View(dbService.usersCollection.AsQueryable<User>().ToList());
        }

        public async Task<IActionResult> Details(Guid? id)
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

            // Retriving Books from db
            IList<Book> rentedBooks = new List<Book>();
            foreach (var item in user.RentedBooks)
            {
                var bookFilter = Builders<Book>.Filter.Eq(book => book.Id, item);
                rentedBooks.Add(await dbService.bookCollection.Find(bookFilter).FirstOrDefaultAsync());
            }            

            // Getting loan history
            var loanFilter = Builders<Loan>.Filter.Eq(loan => loan.UserId, id);
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

            UserProfileViewModel userVM = new UserProfileViewModel()
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                SSN = user.SSN,
                City = user.City,
                Street = user.Street,
                HouseNumber = user.HouseNumber,
                ZipCode = user.ZipCode,
                Username = user.Username,
                Email = user.Email,
                RentedBooks = rentedBooks, // Here, rented books are instances of Book class, NOT Guid
                Approved = user.Approved,
                Banned = user.Banned,
                Loans = loans,         
                Books = books
            };

            return View(userVM);
        }

        public IActionResult Create()
        {
            return View();
        }
        public IActionResult CreateRole()
        {
            return View();
        }

        // User register
        [HttpPost]
        public async Task<IActionResult> Create(User user)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser appUser = new ApplicationUser
                {
                    Name = user.Name,
                    Surname = user.Surname,
                    SSN = user.SSN,
                    City = user.City,
                    Street = user.Street,
                    HouseNumber = user.HouseNumber,
                    ZipCode = user.ZipCode,
                    UserName = user.Username,
                    Email = user.Email,
                    RentedBooks = user.RentedBooks,
                    Approved = true,
                    Banned = false
                };

                IdentityResult result = await userManager.CreateAsync(appUser, user.Password);

                // Add role
                await userManager.AddToRoleAsync(appUser, "Customer");

                if (result.Succeeded)
                {
                    ViewBag.Message = "User created successfully.";
                    return RedirectToAction(nameof(DashboardController.Index), nameof(DashboardController).Replace("Controller", ""), new { area = "Admin" });
                }
                else
                {
                    foreach (IdentityError e in result.Errors)
                    {
                        ModelState.AddModelError("", e.Description);
                        ViewBag.ErrorMessage = e.Description;
                    }
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                    ModelState.AddModelError("", error.ErrorMessage);
                    ViewBag.ErrorMessage = error.ErrorMessage;
                }

            }
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(UserRole userRole)
        {
            if (ModelState.IsValid)
            {

                IdentityResult result = await roleManager.CreateAsync(new ApplicationRole() { Name = userRole.RoleName });
                if (result.Succeeded)
                {
                    ViewBag.Message = "Role created successfully.";
                }
                else
                {
                    foreach (IdentityError e in result.Errors)
                    {
                        ModelState.AddModelError("", e.Description);
                        ViewBag.ErrorMessage = e.Description;
                    }
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }

            }
            return View();
        }

        // GET: Admin/User/Edit/id
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

        // POST: Admin/User/Edit/id
        // Customer can't update Username and password
        [HttpPost]
        public async Task<IActionResult> Edit(Guid? id, User userEdit)
        {            
            if(id == null || userEdit == null)
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
                        .Set(c => c.Email, userEdit.Email);
                
                var result = await dbService.usersCollection.UpdateOneAsync(filter, update);                
                if(result.IsAcknowledged)
                {
                    return RedirectToAction(nameof(DashboardController.Index), nameof(DashboardController).Replace("Controller", ""), new { area = "Admin" });
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
            return View(userEdit);
        }

        public async Task<IActionResult> Delete(Guid? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var filter = Builders<User>.Filter.Eq(user => user.Id, id);
            var result = await dbService.usersCollection.DeleteOneAsync(filter);

            if(result.DeletedCount == 1)
            {
                return RedirectToAction(nameof(DashboardController.Index), nameof(DashboardController).Replace("Controller", ""), new { area = "Admin" });
            }
            else
            {                
                return ViewBag.ErrorMessage = "User Delete Error!";
            }
        }

        public async Task<IActionResult> Approve(Guid? id)
        {
            if(id == null)
            {                
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                var filter = Builders<User>.Filter.Eq(user => user.Id, id);

                var update = Builders<User>.Update.Set(c => c.Approved, true);
                var result = await dbService.usersCollection.UpdateOneAsync(filter, update);

                return RedirectToAction(nameof(DashboardController.Index), nameof(DashboardController).Replace("Controller", ""), new { area = "Admin" });
            }
            else
            {
                // TODO: add error view
                return NotFound();
            }
        }

        public async Task<IActionResult> Ban(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                var filter = Builders<User>.Filter.Eq(user => user.Id, id);

                var update = Builders<User>.Update
                                           .Set(c => c.Banned, true)
                                           .Set(c => c.Approved, false);
                var result = await dbService.usersCollection.UpdateOneAsync(filter, update);

                return RedirectToAction(nameof(DashboardController.Index), nameof(DashboardController).Replace("Controller", ""), new { area = "Admin" });
            }
            else
            {
                // TODO: add error view
                return NotFound();
            }
        }

        public async Task<IActionResult> RemoveBook(Guid userId, Guid bookId)
        {
            if (userId == Guid.Empty && bookId == Guid.Empty)
            {
                return NotFound();
            }

            // Remove book from user 
            var userFilter = Builders<User>.Filter.Eq(user => user.Id, userId);
            User user = await dbService.usersCollection.Find(userFilter).FirstOrDefaultAsync();
            if(user == null)
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
            if(book == null)
            {
                return NotFound();
            }

            book.Available += 1;
            var bookUpdate = Builders<Book>.Update
                                .Set(b => b.Available, book.Available);
            var bookResult = await dbService.bookCollection.UpdateOneAsync(bookFilter, bookUpdate);
            if (!bookResult.IsAcknowledged)
            {
                return NotFound();
            }
            if (userResult.ModifiedCount == 1 && bookResult.ModifiedCount == 1)
            {                
                return RedirectToAction(nameof(UserController.Details), nameof(UserController).Replace("Controller", ""), new { area = "Admin", id = user.Id });
            }
            else
            {
                return ViewBag.ErrorMessage = "Book remove error!";
            }
        }
    }
}
