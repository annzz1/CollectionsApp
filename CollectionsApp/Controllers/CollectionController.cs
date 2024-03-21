using CollectionsApp.Data;
using CollectionsApp.Models;
using CollectionsApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Data;

namespace CollectionsApp.Controllers
{
    public class CollectionController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public CollectionController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Create(string Id, CollectionVM model)
        {
            if (ModelState.IsValid)
            {
                Collection collection = new Collection
                {
                    Name = model.Name,
                    Description = model.Description,
                    category = model.category,
                    
                };

                var user = await _context.Users.Include(u => u.Collections).FirstOrDefaultAsync(u => u.Id == Id);

                if (user != null)
                {
                    if(model.CustomFields!=null && model.CustomFields.Any())
                    {
                        foreach (var cf in model.CustomFields)
                        {
                            var customfield = new CustomField
                            {
                                customFieldType = cf.customFieldType,
                                Value = cf.Value,
                                collection = collection,
                                CollectionId = collection.Id
                            };
                            _context.CustomFields.Add(customfield);
                            collection.CustomFields.Add(customfield);
                        }
                    }
                    collection.appUser = user;
                    collection.AppUserId = user.Id;
                    _context.Collections.Add(collection);
                    user.Collections.Add(collection);
                    var result = await _context.SaveChangesAsync();
                    if (result > 0)
                    {
                        await _userManager.UpdateAsync(user);
                        return RedirectToAction("Profile", "Home", new { Id = Id });
                    }
                    else
                    {
                        ModelState.AddModelError("", "Unsuccessful add attempt!");
                    }
                }
            }

            var collection_ = await _context.Collections.FirstOrDefaultAsync(c => c.AppUserId == Id);
            return View(collection_);
        }
        public async Task<IActionResult> Edit(string Id)
        {
            if (Id == null)
            {
                return NotFound();
            }
            var collection = await _context.Collections.FindAsync(Id);
            

            if (collection == null)
            {
                return NotFound();
            }
            return View(collection);
        }
        [HttpPost]
       
        public async Task<IActionResult> Edit(string Id, [Bind("Name,Description,category")]CollectionVM collection)
        {   if (Id.IsNullOrEmpty())
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var CurrentCollection = _context.Collections.FirstOrDefault(i => i.Id == Id);
                CurrentCollection.Name = collection.Name;
                CurrentCollection.Description = collection.Description;
                CurrentCollection.category = collection.category;
                try
                {

                    _context.Collections.Update(CurrentCollection);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Profile", "Home", new { Id = CurrentCollection.AppUserId });
                    
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Collections.Contains(CurrentCollection))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            
            }
           
            return View(collection);
        }
        [HttpPost]

        public async Task<IActionResult> Delete(string Id)
        {
           
            if (Id.IsNullOrEmpty())
            {
                return NotFound();
            }
            var collection_ = await _context.Collections.FirstOrDefaultAsync(c => c.Id == Id);
            if (collection_ != null)
            {
                _context.Collections.Remove(collection_);
                _context.SaveChanges();

                return RedirectToAction("Profile", "Home", new { Id = collection_.AppUserId });

            }
            else
            {
                return NotFound();
            }
           

        }
        
       

    }
}
