using CollectionsApp.Models;

namespace CollectionsApp.ServiceContracts
{
    
        public interface ISearchService
        {
           public Task<IEnumerable<Item>> SearchItems(string searchTerm);
        }

    
}
