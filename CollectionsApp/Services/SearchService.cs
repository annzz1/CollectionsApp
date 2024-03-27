using CollectionsApp.Data;
using CollectionsApp.Models;
using CollectionsApp.ServiceContracts;
using Microsoft.EntityFrameworkCore;


namespace CollectionsApp.Services
{
    public class SearchService : ISearchService
    {
        private readonly AppDbContext _context;

        public SearchService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Item>> SearchItems(string searchTerm)
        {
            var items = await _context.Items
                .Include(i => i.collection)
                .ThenInclude(c => c.appUser)
                .Where(item => EF.Functions.Contains(item.Name, $"\"{searchTerm}\"") ||
                               EF.Functions.Contains(item.collection.Name, $"\"{searchTerm}\"") ||
                               EF.Functions.Contains(item.collection.appUser.UserName, $"\"{searchTerm}\"")||
                                EF.Functions.Contains(item.collection.appUser.FirstName, $"\"{searchTerm}\"")||
                                 EF.Functions.Contains(item.collection.appUser.LastName, $"\"{searchTerm}\""))
                .ToListAsync();
            
            return items;
        }
    }


}
