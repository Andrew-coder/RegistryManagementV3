using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RegistryManagementV3.Models;
using RegistryManagementV3.Models.Domain;
using RegistryManagementV3.Models.Repository;
using RegistryManagementV3.Services;
using RegistryManagementV3.Services.resources;

namespace RegistryManagementV3.Controllers
{
    public class CatalogController : Controller
    {
        private readonly ICatalogService _catalogService;
        private readonly IUserGroupService _userGroupService;
        private readonly SecurityDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ResourceManagementStrategy _resourceManagementStrategy;

        public CatalogController(ICatalogService catalogService, IUserGroupService userGroupService, SecurityDbContext db, UserManager<ApplicationUser> userManager, ResourceManagementStrategy resourceManagementStrategy)
        {
            _catalogService = catalogService;
            _userGroupService = userGroupService;
            _db = db;
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
        public ActionResult Details(int? id)
        {
            if (id == null)
            { 
                return new StatusCodeResult(400);
            }
            var catalog = _db.Catalogs.Find(id);
            if (catalog == null)
            {
                return new StatusCodeResult(404);
            }
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
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(400);
            }
            var catalog = _db.Catalogs.Find(id);
            if (catalog == null)
            {
                return new StatusCodeResult(404);
            }
            return View(catalog);
        }

        // POST: Catalog/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind("Id,Name")] Catalog catalog)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(catalog).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(catalog);
        }

        // GET: Catalog/Delete/5
        public ActionResult Delete(long? catalogId)
        {
            if (catalogId == null)
            {
                return new StatusCodeResult(400);
            }

            var id = catalogId.GetValueOrDefault();
            var catalog = _db.Catalogs.Find(id);
            if (catalog == null)
            {
                return new StatusCodeResult(404);
            }

            if (!_catalogService.ContainsSubCatalogs(id))
            {
                _catalogService.RemoveCatalog(id);
            }
            return RedirectToAction("Index", "Catalog");
        }
    }
}
