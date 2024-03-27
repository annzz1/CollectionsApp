using CollectionsApp.ServiceContracts;
using Microsoft.AspNetCore.Mvc;

namespace CollectionsApp.Controllers
{
    public class SearchController : Controller
    {
        private readonly ISearchService _searchService;

        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        public async Task<IActionResult> Index(string searchTerm)
        {
            var searchResults = await _searchService.SearchItems(searchTerm);
            
            return View(searchResults);
        }
    }

}
