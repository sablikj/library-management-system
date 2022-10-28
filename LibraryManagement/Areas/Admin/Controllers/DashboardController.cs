using LibraryManagement.Areas.Admin.Models.ViewModels;
using LibraryManagement.Controllers;
using LibraryManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using MongoDB.Driver.Linq;
using MongoDB.Driver;
using LibraryManagement.Models.Entity;

namespace LibraryManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Librarian")]
    public class DashboardController : Controller
    {
        protected DatabaseService dbService;
        public DashboardController(DatabaseService dbService)
        {
           this.dbService = dbService;
        }

        public IActionResult Index()
        {
            DashboardViewModel dashboardViewModel = new DashboardViewModel();
            dashboardViewModel.Books = dbService.bookCollection.AsQueryable<Book>().ToList();
            dashboardViewModel.Users = dbService.usersCollection.AsQueryable<User>().ToList();
            dashboardViewModel.Loans = dbService.loanCollection.AsQueryable<Loan>().ToList();

            Console.WriteLine(dashboardViewModel.Users.Count);

            return View(dashboardViewModel);
        }
    }
}
