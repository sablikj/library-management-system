using LibraryManagement.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LibraryManagement.Models;
using Microsoft.AspNetCore.Authorization;

namespace LibraryManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Librarian")]
    public class UserController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<ApplicationRole> _roleManager;
        public UserController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public IActionResult Create()
        {
            return View();
        }
        public IActionResult CreateRole()
        {
            return View();
        }

        // Used for register -- rework for admin adding customers

        [HttpPost]
        public async Task<IActionResult> Create(User user)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser appUser = new ApplicationUser
                {
                    Name = user.Name,
                    Surname = user.Surname,
                    SNN = user.SNN,
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

                IdentityResult result = await _userManager.CreateAsync(appUser, user.Password);

                // Add role
                await _userManager.AddToRoleAsync(appUser, "Customer");

                if (result.Succeeded)
                {
                    ViewBag.Message = "User created successfully.";
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
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(UserRole userRole)
        {
            if (ModelState.IsValid)
            {

                IdentityResult result = await _roleManager.CreateAsync(new ApplicationRole() { Name = userRole.RoleName });
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
    }
}
