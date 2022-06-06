using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HairDresser1.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using HairDresser1.Models;
using Microsoft.AspNetCore.Authorization;

namespace HairDresser1.Controllers
{
    public class ApplicationUsersController : Controller
    {
        private readonly HairDresserDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IHttpContextAccessor _httpContext;
        public ApplicationUsersController(HairDresserDbContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IHttpContextAccessor httpContext
           )
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _httpContext = httpContext;
        }

        public async Task<ApplicationUser> GetCurrentUser()
        {
            var userId = _httpContext.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            return user;
        }

        //public async Task<IActionResult> Details(string id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var applicationUser = await _userManager.FindByIdAsync(id);
        //    if (applicationUser == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(applicationUser);
        //}

        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Saloon");
        }


        [Route("login")]
        public IActionResult Login()
        {
            return View();
        }
        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> Login(SignInModel model,string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, true);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        return LocalRedirect(returnUrl);
                    }
                    if (_userManager.FindByEmailAsync(model.Email).Result.UserType == "saloon" && !_context.Saloon.Where(x=> x.SaloonOwnerID == _userManager.FindByEmailAsync(model.Email).Result.Id).Any())
                    {
                        TempData["ownerid"] = _userManager.FindByEmailAsync(model.Email).Result.Id;
                        return RedirectToAction("Create", "Saloon");
                    }
                    return RedirectToAction("Index", "Saloon");
                }
                if (result.IsNotAllowed)
                {
                    ModelState.AddModelError("", "Not allowed to login");
                }
                else if (result.IsLockedOut)
                {
                    ModelState.AddModelError("", "Account blocked. Try after some time.");
                }
                else
                {
                    ModelState.AddModelError("", "Your password or email is incorrect. Please try again");
                }
            }
            return View(model);
        }


        [Route("sign-up")]
        [HttpGet]
        public IActionResult Signup()
        {

            return View();
        }


        // POST: ApplicationUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [Route("sign-up")]

        [Route("signup")]

        [HttpPost]
        public async Task<IActionResult> Signup(SignupModel applicationUser)
        {
            if (ModelState.IsValid && applicationUser.Terms)
            {

                var user = new ApplicationUser()
                {
                    FirstName = applicationUser.FirstName,
                    Surname = applicationUser.LastName,
                    UserType = applicationUser.UserType,
                    Email = applicationUser.Email,
                    PhoneNumber = applicationUser.PhoneNumber,
                    UserName = applicationUser.Email
                   
                };

                if (!_roleManager.RoleExistsAsync("saloon").Result)
                {
                    await _roleManager.CreateAsync(new IdentityRole("saloon"));
                }
                if (!_roleManager.RoleExistsAsync("user").Result)
                {
                    await _roleManager.CreateAsync(new IdentityRole("user"));
                }
                
                var result = await _userManager.CreateAsync(user, applicationUser.Password);
                var result2 = await _userManager.AddToRoleAsync(user, applicationUser.UserType);
                if (result.Succeeded && result2.Succeeded)
                {
                    if (applicationUser.UserType == "saloon")
                    {
                        TempData["ownerid"] = user.Id;
                        ModelState.Clear();
                        return RedirectToAction("Create", "Saloon");
                        
                    }
                    else
                    {
                        ModelState.Clear();
                        return RedirectToAction("Index", "Saloon");
                    }
                }
                else if (result.Succeeded && !result2.Succeeded)
                {
                    await _userManager.DeleteAsync(user);
                }
                else if (!result.Succeeded)
                {
                    foreach (var errorMessage in result.Errors)
                    {
                        ModelState.AddModelError("", errorMessage.Description);
                    }

                    return View(applicationUser);
                }
            }
            return View(applicationUser);
        }

        [Authorize(Roles="saloon,user")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id != GetCurrentUser().Result.Id)
            {
                return NotFound();
            }
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await GetCurrentUser();
            if (applicationUser == null)
            {
                return NotFound();
            }
            return View(applicationUser);
        }

        // POST: ApplicationUsers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, ApplicationUser applicationUser)
        {
            if (id != applicationUser.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var user = _userManager.FindByIdAsync(id).Result;
                    if (applicationUser.FirstName != null)
                    {
                        user.FirstName = applicationUser.FirstName;
                    }
                    if (applicationUser.Surname != null)
                    {
                        user.Surname = applicationUser.Surname;
                    }
                    if (applicationUser.Email != null)
                    {
                        user.Email= applicationUser.Email;
                        user.UserName = applicationUser.Email;
                    }
                    if (applicationUser.PhoneNumber != null)
                    {
                        user.PhoneNumber = applicationUser.PhoneNumber;
                    }
                    await _userManager.UpdateAsync(user);
                    await _signInManager.RefreshSignInAsync(user);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApplicationUserExists(applicationUser.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index","Saloon");
            }
            return View(applicationUser);
        }

        [Route("change-password")]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await GetCurrentUser();
                var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    ViewBag.IsSuccess = true;
                    ModelState.Clear();
                    return View();
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

            }
            return View(model);
        }
        private bool ApplicationUserExists(string id)
        {
            return _userManager.FindByIdAsync(id).IsCompleted;
                }
    }
}
