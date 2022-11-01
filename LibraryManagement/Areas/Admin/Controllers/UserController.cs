using LibraryManagement.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LibraryManagement.Models.Entity;
using LibraryManagement.Services;
using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.AspNetCore.Mvc.ModelBinding;

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
            return View(user);
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
    }
}
