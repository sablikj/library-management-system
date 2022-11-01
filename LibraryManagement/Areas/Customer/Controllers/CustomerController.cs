
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
            CustomerViewModel customerVM = new CustomerViewModel();
            Guid customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (customerId == Guid.Empty)
            {
                return NotFound();
            }
            var userFilter = Builders<User>.Filter.Eq(user => user.Id, customerId);
            var loanFilter = Builders<Loan>.Filter.Eq(loan => loan.UserId, customerId);            
            
            // Get current user            
            customerVM.Customer = await dbService.usersCollection.Find(userFilter).FirstOrDefaultAsync();
            if (customerVM.Customer == null)
            {
                return NotFound();
            }

            // Get customers loans
            customerVM.Loans = await dbService.loanCollection.Find(loanFilter).ToListAsync();

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
    }
}
