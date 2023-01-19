using LibraryManagement.Areas.Admin.Controllers;
using LibraryManagement.Controllers;
using LibraryManagement.Models;
using LibraryManagement.Models.Identity;
using LibraryManagement.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Areas.Security.Controllers
{
    [Area("Security")]
    public class AccountController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string? returnUrl)
        {
            LoginViewModel model = new LoginViewModel
            {
                ReturnUrl = returnUrl             
            };

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel loginVM)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser appUser = await _userManager.FindByNameAsync(loginVM.Username);
                if (appUser != null)
                {
                    Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(appUser, loginVM.Password, false, false);
                    if (result.Succeeded)
                    {
                        // Admin redirected to dashboard
                        HttpContext.Session.SetString("loginVM.Username", loginVM.Username);
                        var roles = await _userManager.GetRolesAsync(appUser);
                        if (roles.Contains("Librarian"))
                        {
                            return RedirectToAction(nameof(DashboardController.Index), nameof(DashboardController).Replace("Controller", ""), new { area = "Admin" });
                        }
                        return RedirectToAction(nameof(HomeController.Index), nameof(HomeController).Replace("Controller", ""), new { area = "" });
                    }                    
                }
                ModelState.AddModelError(nameof(loginVM.Username), "Login failed: Invalid username or password.");
            }
            return View(loginVM);
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), nameof(HomeController).Replace("Controller", ""), new { area = "" });
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerVM)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser appUser = new ApplicationUser
                {
                    Name = registerVM.Name,
                    Surname = registerVM.Surname,
                    SSN = registerVM.SNN,
                    City = registerVM.City,
                    Street = registerVM.Street,
                    HouseNumber = registerVM.HouseNumber,
                    ZipCode = registerVM.ZipCode,
                    UserName = registerVM.Username,
                    Email = registerVM.Email,
                    RentedBooks = {},
                    Approved = false,
                    Banned = false,
                    BannedDate = DateTime.UnixEpoch
                };

                IdentityResult result = await _userManager.CreateAsync(appUser, registerVM.Password);

                // Add role
                await _userManager.AddToRoleAsync(appUser, "Customer");

                if (result.Succeeded)
                {
                    LoginViewModel loginVM = new LoginViewModel()
                    {
                        Username = registerVM.Username,
                        Password = registerVM.Password
                    };

                    return await Login(loginVM);
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
            return View(registerVM);
        }
    }
}
