using CollectionsApp.Data;
using CollectionsApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace CollectionsApp.Controllers
{
    
    public class HomeController : Controller
    {
       
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly AppDbContext _context;

        public HomeController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;

        }
        public IActionResult Index()
        {
            return View();
        }
        
       
        [HttpGet]
        
        public async Task<IActionResult> Profile(string Id)
        {
            var user =  _userManager.Users.Include(u => u.Collections).FirstOrDefault(u => u.Id == Id);
            if (user == null) { return NotFound(); }
            return View(user); 
 
        }
        public async Task<IActionResult> AdminPage()
        {
            return View(await _userManager.Users.ToListAsync<AppUser>());
        }
    }
}
