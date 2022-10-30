using LibraryManagement.Areas.Customer.Models.ViewModels;
using LibraryManagement.Models.Entity;
using LibraryManagement.Models.Identity;
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
        
        public CustomerController(DatabaseService dbService)
        {
            this.dbService = dbService;           
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
    }
}
