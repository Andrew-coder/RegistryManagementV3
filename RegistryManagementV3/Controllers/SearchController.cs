using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RegistryManagementV3.Models;
using RegistryManagementV3.Models.Domain;
using RegistryManagementV3.Services;
using RegistryManagementV3.Services.resources;

namespace RegistryManagementV3.Controllers
{
    public class SearchController : Controller
    {
        private readonly ITagService _tagService;
        private readonly ISearchService _searchService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ResourceManagementStrategy _resourceManagementStrategy;

        public SearchController(ITagService tagService, ISearchService searchService, UserManager<ApplicationUser> userManager, ResourceManagementStrategy resourceManagementStrategy)
        {
            _tagService = tagService;
            _searchService = searchService;
            _userManager = userManager;
            _resourceManagementStrategy = resourceManagementStrategy;
        }

        // GET: /Search
        [HttpPost]
        public async Task<ActionResult> SearchQuery(SearchViewModel query)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _userManager.FindByIdAsync(userId).Result;
            var roles = await _userManager.GetRolesAsync(user);
            var managementService = _resourceManagementStrategy.FindService(roles.FirstOrDefault());

            var resources = managementService.SearchResourcesByQuery(query.Query, user);
            return View("SearchResults", resources);
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