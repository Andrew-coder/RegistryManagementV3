using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RegistryManagementV3.Models.Domain;
using RegistryManagementV3.Services;

namespace RegistryManagementV3.Controllers
{
    public class SearchController : Controller
    {
        private readonly ITagService _tagService;
        private readonly ISearchService _searchService;
        private UserManager<ApplicationUser> _userManager;

        public SearchController(ITagService tagService, ISearchService searchService, UserManager<ApplicationUser> userManager)
        {
            _tagService = tagService;
            _searchService = searchService;
            _userManager = userManager;
        }

//        // GET: /Search
//        public ActionResult SearchResources(SearchViewModel searhViewModel)
//        {
//            if (String.IsNullOrEmpty(searhViewModel.Tags))
//            {
//                return RedirectToAction("Index", "Catalog");
//            }
//            var tagNames = new Collection<string>(searhViewModel.Tags.Split(','));
//            var isAdmin = User.IsInRole("Admin");
//            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
//            var user = _userManager.FindByIdAsync(identity.Identity.GetUserId());
//            var resources = _searchService.SearchResourcesByTags(tagNames, user, isAdmin);
//            return View("SearchResults", resources);
//        }
    }
}