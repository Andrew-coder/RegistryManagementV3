using System.Collections.ObjectModel;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RegistryManagementV3.Models;
using RegistryManagementV3.Models.Domain;
using RegistryManagementV3.Services;

namespace RegistryManagementV3.Controllers
{
    public class SearchController : Controller
    {
        private readonly ITagService _tagService;
        private readonly ISearchService _searchService;
        private readonly UserManager<ApplicationUser> _userManager;

        public SearchController(ITagService tagService, ISearchService searchService, UserManager<ApplicationUser> userManager)
        {
            _tagService = tagService;
            _searchService = searchService;
            _userManager = userManager;
        }

        // GET: /Search
        [HttpPost]
        public ActionResult SearchQuery(SearchViewModel query)
        {

            return View("SearchResults", null);
        }
        
        // GET: /Search with filters
        [HttpPost]
        public ActionResult SearchFilters(SearchFilterViewModel searchFilterViewModel)
        {
            if (string.IsNullOrEmpty(searchFilterViewModel.Tags))
            {
                return RedirectToAction("Index", "Catalog");
            }
            var tagNames = new Collection<string>(searchFilterViewModel.Tags.Split(','));
            var isAdmin = User.IsInRole("Admin");
            
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _userManager.FindByIdAsync(userId).Result;
            
            var resources = _searchService.SearchResourcesByTags(tagNames, user, isAdmin);
            return View("SearchResults", resources);
        }
    }
}