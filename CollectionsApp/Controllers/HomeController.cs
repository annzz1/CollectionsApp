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
        public async Task<IActionResult> Index()
        {
            var collections = await _context.Collections.Include(i=>i.appUser).Include(i => i.Items).Include(x => x.CustomFields).ToListAsync();
            
            // Calculate item counts for each collection
            var collectionItemCounts = collections
                .Select(c => new
                {
                    Collection = c,
                    ItemCount = c.Items.Count
                });

            // Sort collections based on item count in descending order
            var sortedCollections = collectionItemCounts
                .OrderByDescending(ci => ci.ItemCount)
                .Select(ci => ci.Collection)
                .Take(5)
                .ToList();

            return View(sortedCollections);
        }
        
       
        [HttpGet]
        
        public async Task<IActionResult> Profile(string Id)
        {

            var user = _userManager.Users.Include(u => u.Collections).ThenInclude(c => c.CustomFields).FirstOrDefault(u => u.Id == Id);
            if (user == null) { return NotFound(); }
            return View(user); 
 
        }
        public async Task<IActionResult> AdminPage()
        {
            return View(await _userManager.Users.ToListAsync<AppUser>());
        }
    }
}
