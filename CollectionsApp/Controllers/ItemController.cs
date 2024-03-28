using CollectionsApp.Data;
using CollectionsApp.Enums;
using CollectionsApp.Models;
using CollectionsApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
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
        public async Task<IActionResult> Index(string Id, string sortOrder, string searchString)
        {
            // Fetch collection data
            var collection_ = await _context.Collections.Include(i=> i.appUser)
                .Include(x => x.Items)
                .Include(x => x.CustomFields)
                .FirstOrDefaultAsync(i => i.Id == Id);

            // Fetch items data
            var itemsQuery = _context.Items
                .Include(x => x.ItemCustomFieldVals)
                .Where(u => u.CollectionId == Id);

            // Apply search filter if a search string is provided
            if (!string.IsNullOrEmpty(searchString))
            {
                itemsQuery = itemsQuery.Where(i => i.Name.Contains(searchString) || i.Tags.Contains(searchString));
            }

            // Apply sorting based on sort column and direction
            switch (sortOrder)
            {
                case "Name":
                    itemsQuery = itemsQuery.OrderBy(i => i.Name);
                    break;
                case "name_desc":
                    itemsQuery = itemsQuery.OrderByDescending(i => i.Name);
                    break;
                case "Tags":
                    itemsQuery = itemsQuery.OrderBy(i => i.Tags);
                    break;
                case "tags_desc":
                    itemsQuery = itemsQuery.OrderByDescending(i => i.Tags);
                    break;
                // Add more cases for other columns if needed
                default:
                    itemsQuery = itemsQuery.OrderBy(i => i.Name); // Default sorting by Name in ascending order
                    break;
            }

            // Execute the query
            var items = await itemsQuery.ToListAsync();

            // Populate the view model
            var itemPageVM = new ItemPageVM
            {
                collection = collection_,
                items = items
            };

            // Set ViewBag for sorting parameters
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParam = sortOrder == "Name" ? "name_desc" : "Name";
            ViewBag.TagsSortParam = sortOrder == "Tags" ? "tags_desc" : "Tags";
            // Add more ViewBag parameters for other columns if needed

            return View(itemPageVM);
        }
        [HttpGet]
        public async Task<IActionResult> ItemPage(string Id)
        {
            if (string.IsNullOrWhiteSpace(Id)) { return NotFound(); }
            var item = await _context.Items.Include(i=>i.ItemCustomFieldVals).Include(x=>x.Likes).Include(y=>y.Comments).FirstOrDefaultAsync(x=> x.Id==Id);
            var currentUser = await _userManager.GetUserAsync(User);

            // Check if the current user has liked the item
            bool likedByCurrentUser = item.Likes.Any(l => l.AppUserId == currentUser.Id);

            // Pass the item and like status to the view
            ViewBag.LikedByCurrentUser = likedByCurrentUser;
            return View(item);
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
              if(cf.customFieldType== CustomFieldTypes.Logical)
                {
                    ItemsVM.customs[cf.Label] = "False"; // Default value for other types
                }
                else
                {
                    ItemsVM.customs[cf.Label] = ""; // Default value for other types
                }
                
                    
                
                ItemsVM.customFieldTypes[cf.Label] = cf.customFieldType.ToString();
               
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
                            Value = model.customs[cf.Label],
                            Label = cf.Label
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

        public async Task<IActionResult> Delete(string Id)
        {
          
            if (Id.IsNullOrEmpty())
            {
                return BadRequest();
            }
            
                var item = await _context.Items
                .Include(x => x.ItemCustomFieldVals).Include(x=>x.Likes).Include(x=>x.Comments)
                .FirstOrDefaultAsync(u => u.Id == Id);
                if (item != null)
                {
                    _context.CustomFieldsValues.RemoveRange(item.ItemCustomFieldVals);
                if (item.Comments.Any())
                {
                    _context.Comments.RemoveRange(item.Comments);
                }
                if (item.Likes.Any())
                {
                    _context.Likes.RemoveRange(item.Likes);
                }
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
            var item = await _context.Items.Include(x => x.ItemCustomFieldVals).FirstOrDefaultAsync(u => u.Id == Id);
            if (item == null)
            {
                return NotFound();
            }
            var edititemVM = new EditItemVM
            {
                Id = item.Id,
                Name = item.Name,
                Tags = item.Tags
            };
            foreach (var cf in item.ItemCustomFieldVals)
            {
                edititemVM.ItemCustomVals[cf.Label] = cf.Value;
            }
            return View(edititemVM);
           
        }
        [HttpPost]

        public async Task<IActionResult> Edit(string Id,EditItemVM item)
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
        [HttpPost]
        public async Task<IActionResult> AddLike(string Id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var item = await _context.Items
                .Include(i => i.collection)
                .ThenInclude(x => x.appUser)
                .FirstOrDefaultAsync(t => t.Id == Id);


            // Create a new like
            var newLike = new Like
            {
                ItemId = Id,
                 AppUserId = currentUser.Id,
                 appuser = currentUser,
                Item = item
            };

            // Add the like to the database
            _context.Likes.Add(newLike);
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                return RedirectToAction("ItemPage", "Item", new { Id = Id });
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(string Id, string content)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var item = await _context.Items.Include(i => i.collection).ThenInclude(x => x.appUser).FirstOrDefaultAsync(t => t.Id == Id);
            // Create a new comment
            var newComment = new Comment
            {
                itemId = Id,
                Item = item,
                Content = content,
                appuser = currentUser,
                AppUserId = currentUser.Id
              
            };

            
            _context.Comments.Add(newComment);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return RedirectToAction("ItemPage", "Item", new { Id = item.Id });
            }
            else
            {
                return NotFound();
            }
            
            
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteComment(string commentId, string itemId)
        {
            var comment = await _context.Comments.Include(i => i.appuser).Include(i => i.appuser).FirstOrDefaultAsync(x => x.Id == commentId);

            if (comment == null)
            {
                return NotFound();
            }
            if (comment.appuser.UserName != User.Identity.Name)
            {
                return Forbid();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("ItemPage", "Item", new { Id = itemId });
        }
        [HttpPost]
        public async Task<IActionResult> RemoveLike(string Id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            // Find the like to remove
            var likeToRemove = await _context.Likes.Include(x=> x.Item).FirstOrDefaultAsync(l => l.ItemId == Id && l.AppUserId == currentUser.Id);

            if (likeToRemove != null)
            {
                // Remove the like from the database
                _context.Likes.Remove(likeToRemove);
              
            }
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return RedirectToAction("ItemPage", "Item", new { Id = Id });
            }
            else
            {
                return BadRequest();
            }

          
        }
    }
}
