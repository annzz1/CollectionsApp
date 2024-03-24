using CollectionsApp.Data;
using CollectionsApp.Models;
using CollectionsApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CollectionsApp.Controllers
{
    public class ItemController : Controller
    {

        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public ItemController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<IActionResult> Index(string Id)
        {
            var collection_ = await _context.Collections.Include(x => x.Items).Include(x=> x.CustomFields).FirstOrDefaultAsync(i => i.Id == Id);
            var items = await _context.Items
            .Include(x => x.ItemCustomFieldVals)
            .Where(u => u.CollectionId == Id)
            .ToListAsync();
            var itemPageVM = new ItemPageVM
            {
                collection = collection_,
                items = items
            };
            return View(itemPageVM);
        }
        [HttpGet]
        public async Task<IActionResult> Create(string Id)
        {
            var ItemsVM = new ItemVM();
            if (Id == null)
            {
                return NotFound();
            }
            var collection = await _context.Collections
            .Include(u => u.CustomFields)
            .FirstOrDefaultAsync(u => u.Id == Id);

            if (collection == null)
            {
                return NotFound();
            }
            foreach (var cf in collection.CustomFields)
            {
                ItemsVM.customs[cf.Label] = " ";
            }

            return View(ItemsVM);

        }
        [HttpPost]
        public async Task<IActionResult> Create(string Id, ItemVM model)
        {
            if (ModelState.IsValid)
            {
                Item item = new Item
                {
                    Name = model.Name,
                    Tags = model.Tags

                };
                var collection = await _context.Collections.Include(c => c.CustomFields).FirstOrDefaultAsync(c => c.Id == Id);
                if (collection != null)
                {
                    foreach (var cf in collection.CustomFields)
                    {
                        var cfValue = new ItemCustomFieldVal
                        {
                            Item = item,
                            ItemId = item.Id,
                            CustomField = cf,
                            CustomFieldId = cf.Id,
                            Value = model.customs[cf.Label]
                        };
                        item.ItemCustomFieldVals.Add(cfValue);
                        cf.CustomFieldVals.Add(cfValue);
                        _context.CustomFieldsValues.Add(cfValue);
                    }
                    item.collection = collection;
                    item.CollectionId = collection.Id;

                    _context.Items.Add(item);
                    collection.Items.Add(item);
                    _context.Collections.Update(collection);
                    var result = await _context.SaveChangesAsync();
                    if (result > 0)
                    {
                       
                        return RedirectToAction("Index", "Item", new { Id = Id });
                    }
                    else
                    {
                        ModelState.AddModelError("", "Unsuccessful add attempt!");
                    }
                }

               
            }
            return View(model);
        }

        [HttpPost]

        public async Task<IActionResult> Delete(string itemIds)
        {
            List<string> ItemIds = itemIds.Split(',').ToList();
            if (ItemIds.IsNullOrEmpty())
            {
                return BadRequest();
            }
            foreach (var id in ItemIds)
            {
                var item = await _context.Items
                .Include(x => x.ItemCustomFieldVals)
                .FirstOrDefaultAsync(u => u.CollectionId == id);
                if (item != null)
                {
                    _context.CustomFieldsValues.RemoveRange(item.ItemCustomFieldVals);
                    _context.Items.Remove(item);
                    _context.SaveChanges();
                    return RedirectToAction("Index", "Item", new { Id = item.CollectionId });

                }
                else
                {
                    return NotFound();
                }
            }
            return RedirectToAction("Index", "Item");


        }
        public async Task<IActionResult> Edit(string Id)
        {
            if (Id == null)
            {
                return NotFound();
            }
            var item = await _context.Items.FindAsync(Id);


            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }
        [HttpPost]

        public async Task<IActionResult> Edit(string Id, ItemVM item)
        {
            if (Id.IsNullOrEmpty())
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var CurrentItem = _context.Items.FirstOrDefault(i => i.Id == Id);
                CurrentItem.Name = item.Name;
                CurrentItem.Tags = item.Tags;
                
                try
                {

                    _context.Items.Update(CurrentItem);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Item", new { Id = CurrentItem.CollectionId });

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Items.Contains(CurrentItem))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

            }
            return View(item);
        }
    }
}
