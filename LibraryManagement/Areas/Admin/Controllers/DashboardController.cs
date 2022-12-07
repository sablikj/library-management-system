using LibraryManagement.Controllers;
using LibraryManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using LibraryManagement.Models.Entity;
using LibraryManagement.Areas.Admin.Models.ViewModels;
using System.Text.Json;
using System.IO;
using MongoDB.Bson;
using System.Text;
using static System.Net.WebRequestMethods;

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
            // TODO: Load only nesessary data (too slow rn)
            CompleteViewModel completeVM = new CompleteViewModel();
            DashboardViewModel dashboardVM = new DashboardViewModel();         
            
            completeVM.Books = dbService.bookCollection.AsQueryable<Book>().ToList();
            completeVM.Users = dbService.usersCollection.AsQueryable<User>().ToList();
            completeVM.Loans = dbService.loanCollection.AsQueryable<Loan>().ToList();

            dashboardVM.completeVM = completeVM;
            return View(dashboardVM);
        }

        public async Task<IActionResult> Export()
        {
            CompleteViewModel completeVM = new CompleteViewModel();
            completeVM.Books = dbService.bookCollection.AsQueryable<Book>().ToList();
            completeVM.Users = dbService.usersCollection.AsQueryable<User>().ToList();
            completeVM.Loans = dbService.loanCollection.AsQueryable<Loan>().ToList();

            if (completeVM != null)
            {
                string jsonData = JsonSerializer.Serialize(completeVM);
                var fileBytes = Encoding.ASCII.GetBytes(jsonData);
                return new FileContentResult(fileBytes, "text/plain")
                {
                    FileDownloadName = "export.json"
                };                            
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Import(IFormFile importData)
        {            
            if (importData.Length > 0)
            {
                string fileContent = null;
                using (var reader = new StreamReader(importData.OpenReadStream()))
                {
                    fileContent = reader.ReadToEnd();
                }
                if(fileContent != null)
                {
                    CompleteViewModel data = Newtonsoft.Json.JsonConvert.DeserializeObject<CompleteViewModel>(fileContent);

                    // Inserting objs one by one to allow duplicate handling
                    if(data.Users != null)
                    {
                        foreach(var user in data.Users)
                        {
                            try
                            {
                                // Add user to DB
                                await dbService.usersCollection.InsertOneAsync(user);
                            }
                            catch (Exception e)
                            {
                                continue;
                            }
                        }                     
                    }
                    else if (data.Books != null)
                    {
                        foreach (var book in data.Books)
                        {
                            try
                            {
                                // Add book to DB
                                await dbService.bookCollection.InsertOneAsync(book);
                            }
                            catch (Exception e)
                            {
                                continue;
                            }
                        }
                    }
                    if (data.Loans != null)
                    {
                        foreach (var loan in data.Loans)
                        {
                            try
                            {
                                // Add loan to DB
                                await dbService.loanCollection.InsertOneAsync(loan);
                            }
                            catch (Exception e)
                            {
                                continue;
                            }
                        }
                    }
                    return RedirectToAction(nameof(DashboardController.Index), nameof(DashboardController).Replace("Controller", ""), new { area = "Admin" });
                }                
            }
            return NotFound();
        }
    }
}
