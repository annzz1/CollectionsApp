using CollectionsApp.Data;
using CollectionsApp.Models;
using CollectionsApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;

namespace CollectionsApp.Controllers
{
    public class UserController : Controller
    {
        private readonly SignInManager<AppUser> signInManager;
        private readonly UserManager<AppUser> userManager;
        private readonly AppDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        

        public UserController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, AppDbContext context, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            _context = context;
            _roleManager = roleManager;
           
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            var roles = new[] { "Admin", "Member" };
            if (ModelState.IsValid)
            {
                AppUser user = new()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    UserName = model.UserName,
                    Email = model.Email,
                    Collections = new List<Collection>()
                 };


            
                foreach(var role in roles)
                {
                    if(!await _roleManager.RoleExistsAsync(role))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(role));
                    }
                  
                }
                var result = await userManager.CreateAsync(user, model.Password!);
                if (_context.Users.Count() <= 5)
                {
                    model.Role = "Admin";
                    await userManager.AddToRoleAsync(user, model.Role);
                }
                else
                {
                    model.Role = "Member";
                    await userManager.AddToRoleAsync(user, model.Role);
                }
                await userManager.UpdateAsync(user);
                
                if (result.Succeeded && User !=null)
                {
                    
                  
                    await signInManager.SignInAsync(user, false);
                    _context.SaveChanges();

                    return RedirectToAction("Index", "Home");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

            }
            return View(model);
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {

                var result = await signInManager.PasswordSignInAsync(model.Username!, model.Password!, model.RememberMe, false);
                var user = await userManager.FindByNameAsync(model.Username);
                if (user == null)
                {
                    ModelState.AddModelError("", "You do not have an account! Please register first!");
                }
                if (result.Succeeded)
                {
                   
                    return RedirectToAction("Index", "Home");
                }
                else if (result.IsLockedOut)
                {
                    ModelState.AddModelError("", "You are locked out!");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid login attempt");
                }


            }
            _context.SaveChanges();
            return View(model);
        }
        public async Task<IActionResult> Edit(string Id)
        {
            if (Id == null)
            {
                return NotFound();
            }
            var user = await userManager.Users.FirstOrDefaultAsync(x => x.Id == Id);


            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }
        [HttpPost]

        public async Task<IActionResult> Edit(string Id, [Bind("FirstName,LastName,UserName,Email")] UserVM user)
        {
            if (Id.IsNullOrEmpty())
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var Currentuser = await userManager.Users.FirstOrDefaultAsync(x => x.Id == Id);
                Currentuser.FirstName = user.FirstName;
                Currentuser.LastName = user.LastName;
                Currentuser.UserName = user.UserName;
                Currentuser.Email = user.Email;
                try
                {

                    await userManager.UpdateAsync(Currentuser);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Profile", "Home", new { Id = Currentuser.Id });

                }
                catch (DbUpdateConcurrencyException)
                { }
                

            }

            return View(user);
        }
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            
            return RedirectToAction("Index", "Home");
        }

    }

}
