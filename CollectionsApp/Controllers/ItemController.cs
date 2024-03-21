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
        public IActionResult Index(string Id)
        {
            var collection = _context.Collections.Include(u => u.Items).FirstOrDefault(u => u.Id == Id);
           
            return View(collection);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(string Id,ItemVM model)
        {    if (ModelState.IsValid)
            {
                Item item = new Item
                {
                    Name = model.Name,
                    Tags = model.Tags

                };

                var collection = await _context.Collections.FirstOrDefaultAsync(i => i.Id == Id);
                if (collection != null)
                {
                    item.collection = collection;
                    item.CollectionId = collection.Id;
                    _context.Items.Add(item);
                    collection.Items.Add(item);
                    var result = await _context.SaveChangesAsync();
                    if (result > 0)
                    {
                        _context.Collections.Update(collection);
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

        public async Task<IActionResult> Delete(string Id)
        {

            if (Id.IsNullOrEmpty())
            {
                return NotFound();
            }
            var item = await _context.Items.FirstOrDefaultAsync(c => c.Id == Id);
            if (item != null)
            {
                _context.Items.Remove(item);
                _context.SaveChanges();

                return RedirectToAction("Index", "Item", new { Id = item.CollectionId });

            }
            else
            {
                return NotFound();
            }


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

        public async Task<IActionResult> Edit(string Id, [Bind("Name,Tags")] ItemVM item)
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
