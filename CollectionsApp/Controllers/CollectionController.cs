using CollectionsApp.Data;
using CollectionsApp.Models;
using CollectionsApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Data;
using System.Security.Claims;

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
                                Label = cf.Label,
                                collection = collection,
                                CollectionId = collection.Id
                            };
                            _context.CustomFields.Add(customfield);
                           // collection.CustomFields.Add(customfield);
                        }
                    }
                    collection.appUser = user;
                    collection.AppUserId = user.Id;
                    _context.Collections.Add(collection);
                    //user.Collections.Add(collection);
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
        [HttpGet]
        public async Task<IActionResult> Edit(string ids)
        {
            if (ids == null)
            {
                return NotFound();
            }
            List<string> Ids = ids.Split(',').ToList();
            foreach (var id in Ids)
            {
                var collection = await _context.Collections.Include(x => x.CustomFields).FirstOrDefaultAsync(c => c.Id == id);
                if (collection != null)
                {
                    var collectionVM = new CollectionVM
                    {
                        Id = collection.Id,
                        Name = collection.Name,
                        category = collection.category,
                        Description = collection.Description,
                        CustomFields = collection.CustomFields.Select(cf => new CustomFieldVM
                        {
                            Id = cf.Id,
                            Label = cf.Label,
                            customFieldType = cf.customFieldType

                        }).ToList()

                    };
                    return View(collectionVM);
                }
                else
                {
                    return NotFound();
                }
            }
            return View();

        }
        [HttpPost]

        public async Task<IActionResult> Edit(string Id, CollectionVM collectionvm)
        {
            if (Id.IsNullOrEmpty())
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var CurrentCollection = await _context.Collections.Include(c => c.CustomFields).FirstOrDefaultAsync(c => c.Id == Id);
                CurrentCollection.Name = collectionvm.Name;
                CurrentCollection.Description = collectionvm.Description;
                CurrentCollection.category = collectionvm.category;

                foreach (var cfvm in collectionvm.CustomFields.Where(vmcf => vmcf.Id != string.Empty)) // Filter out fields with Id = 0 (new)
                {
                    var inCustomField = CurrentCollection.CustomFields.FirstOrDefault(cf => cf.Id == cfvm.Id);

                    if (inCustomField != null)
                    {
                        inCustomField.Label = cfvm.Label;
                        inCustomField.customFieldType = cfvm.customFieldType;

                        _context.CustomFields.Update(inCustomField);

                    }

                }

                foreach (var cfvm in collectionvm.CustomFields.Where(vmcf => vmcf.Id == string.Empty))
                {
                    CurrentCollection.CustomFields.Add(new CustomField
                    {
                        Label = cfvm.Label,
                        customFieldType = cfvm.customFieldType,
                        CollectionId = CurrentCollection.Id,
                        collection = CurrentCollection
                    });
                }
                var DeletecustomFields = CurrentCollection.CustomFields.Where(cf => cf.Id != string.Empty && !collectionvm.CustomFields.Any(cfvm => cfvm.Id == cf.Id)).ToList();
                _context.CustomFields.RemoveRange(DeletecustomFields);

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

            return View(collectionvm);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(List<string> ids)
        {
            if (ids.IsNullOrEmpty())
            {
                return NotFound();
            }

            // Retrieve the user ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            foreach (var id in ids)
            {
                var collection_ = await _context.Collections.FirstOrDefaultAsync(c => c.Id == id && c.AppUserId == userId);

                if (collection_ != null)
                {
                    _context.Collections.Remove(collection_);
                }
                else
                {
                    return NotFound();
                }
            }

            await _context.SaveChangesAsync();

            // Redirect to the Profile action in HomeController with userId
            return RedirectToAction("Profile", "Home", new { Id = userId });
        }




    }
}
