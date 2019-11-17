using System;
using System.Collections.Generic;
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
    public class CatalogController : Controller
    {
        private readonly ICatalogService _catalogService;
        private readonly IUserGroupService _userGroupService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ResourceManagementStrategy _resourceManagementStrategy;

        public CatalogController(ICatalogService catalogService, IUserGroupService userGroupService, UserManager<ApplicationUser> userManager, ResourceManagementStrategy resourceManagementStrategy)
        {
            _catalogService = catalogService;
            _userGroupService = userGroupService;
            _userManager = userManager;
            _resourceManagementStrategy = resourceManagementStrategy;
        }

        // GET: Catalog
        public async Task<ActionResult> Index(long? catalogId)
        {
            
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _userManager.FindByIdAsync(userId).Result;
            var roles = await _userManager.GetRolesAsync(user);
            
            var managementService = _resourceManagementStrategy.FindService(roles.FirstOrDefault());
            var catalogs = managementService.GetCatalogsByParentCatalog(catalogId, user);
            var resources = managementService.GetResourcesByParentCatalog(catalogId, user);
            
            var tuple = new Tuple<IList<Catalog>, IList<Resource>, long?>(catalogs, resources, catalogId);
            return View(tuple);
        }

        // GET: Catalog/Details/5
        public ActionResult Details(long id)
        {
            var catalog = _catalogService.GetById(id);
            return View(catalog);
        }
        
        // GET: Catalog/Create
        public ActionResult Create(int? id)
        {
            var userGroups = _userGroupService.GetAllUserGroups();
            var tuple = new Tuple<CatalogViewModel, List<UserGroup>, long?>(new CatalogViewModel {SecurityLevel = 5}, userGroups, id);
            return View(tuple);
        }

        // POST: Catalog/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CatalogViewModel catalogViewModel)
        {
            var catalog = new Catalog
            {
                SuperCatalogId = catalogViewModel.CatalogId, Name = catalogViewModel.Name,
                SecurityLevel = catalogViewModel.SecurityLevel
            };
            _catalogService.SaveCatalog(catalog);
            return RedirectToAction("Index");
        }

        // GET: Catalog/Edit/5
        public ActionResult Edit(long id)
        {
            var catalog = _catalogService.GetById(id);
            return View(catalog);
        }

        // POST: Catalog/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CatalogViewModel catalog)
        {
            if (ModelState.IsValid)
            {
//                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(catalog);
        }

        // GET: Catalog/Delete/5
        public ActionResult Delete(long catalogId)
        {
            if (!_catalogService.ContainsSubCatalogs(catalogId))
            {
                _catalogService.RemoveCatalog(catalogId);
            }
            return RedirectToAction("Index", "Catalog");
        }
    }
}
