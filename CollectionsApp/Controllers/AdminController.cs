using CollectionsApp.Data;
using CollectionsApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;

namespace CollectionsApp.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
       
        private readonly AppDbContext _context;

        public AdminController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;

        }
      

        [HttpPost]
        public async Task<IActionResult>AddAdmin(string userIds)
        {
            List<string> UserIds = userIds.Split(',').ToList();
            if (UserIds.IsNullOrEmpty())
            {
                return BadRequest();
            }
            foreach (var id in UserIds)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user != null)
                {
                    await _userManager.RemoveFromRoleAsync(user, "Member");
                    await _userManager.AddToRoleAsync(user, "Admin");
                    await _userManager.UpdateAsync(user);
                }

            }
            _context.SaveChanges();
            return RedirectToAction("AdminPage","Home");
            
        }
       
        [HttpPost]
        public async Task<IActionResult> RemoveAdmin(string userIds)
        {
            List<string> UserIds = userIds.Split(',').ToList();
            if (UserIds.IsNullOrEmpty())
            {
                return BadRequest();
            }
            foreach (var id in UserIds)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user != null)
                {
                    await _userManager.RemoveFromRoleAsync(user, "Admin");
                    await _userManager.AddToRoleAsync(user, "Member");
                    await _userManager.UpdateAsync(user);
                    _context.SaveChanges();
                }

            }
            var currentUser = await _userManager.GetUserAsync(User);
            
            if (currentUser!=null && UserIds.Contains(currentUser.Id))
            {
            
                await _signInManager.SignOutAsync();
                return RedirectToAction("Login","User");
            }


            return RedirectToAction("AdminPage", "Home");


        }
        [HttpPost]
        public async Task<IActionResult> Block(string Id)
        {

            var currentUser = await _userManager.GetUserAsync(User);
            var user = await _userManager.FindByIdAsync(Id);
            if (user != null)
            {
                user.IsActive = false;
                await _userManager.UpdateAsync(user);
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
            }
            if (currentUser != null && (currentUser.Id == Id))
            {

                await _signInManager.SignOutAsync();
                _context.SaveChanges();
                return RedirectToAction("Index", "Home");
            }

            _context.SaveChanges();
            return RedirectToAction("AdminPage","Home");
        }
        [HttpPost]
        public async Task<IActionResult>UnBlock(string Id)
        {
            if(Id == null)
            {
                return BadRequest();
            }
            var user = await _userManager.FindByIdAsync(Id);
            if (user != null)
            {
                user.IsActive = true;
                await _userManager.SetLockoutEndDateAsync(user, null);
                await _userManager.UpdateAsync(user);
               
            }
            _context.SaveChanges();
            return RedirectToAction("AdminPage","Home");

        }
        [HttpPost]
        public async Task<IActionResult> Delete(string Id)
        {
            if (Id == null)
            {
                return BadRequest();
            }
            var curuser = await _userManager.GetUserAsync(User);
            var currentUser = await _context.Users.Include(i => i.Comments).Include(i => i.Likes).Include(i =>i.Collections).ThenInclude(c=>c.Items).FirstOrDefaultAsync(i => i.Id == curuser.Id);
            var user = await _context.Users.Include(i => i.Comments).Include(i => i.Likes).FirstOrDefaultAsync(i => i.Id ==Id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

            }
            if(currentUser != null&& currentUser.Id==Id) 
            {
                await _signInManager.SignOutAsync();
              
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("AdminPage", "Home");

        }
    }
}
