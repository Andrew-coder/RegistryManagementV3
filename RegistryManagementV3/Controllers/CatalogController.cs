using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RegistryManagementV3.Models;
using RegistryManagementV3.Models.Domain;
using RegistryManagementV3.Models.Repository;
using RegistryManagementV3.Services;

namespace RegistryManagementV3.Controllers
{
//    [ClaimsAuthorize(AccountStatus = AccountStatus.Approved)]
    public class CatalogController : Controller
    {
        private readonly ICatalogService _catalogService;
        private readonly IResourceService _resourceService;
        private readonly IUserGroupService _userGroupService;
        private readonly SecurityDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public CatalogController(ICatalogService catalogService, IResourceService resourceService, IUserGroupService userGroupService, SecurityDbContext db, UserManager<ApplicationUser> userManager)
        {
            _catalogService = catalogService;
            _resourceService = resourceService;
            _userGroupService = userGroupService;
            _db = db;
            _userManager = userManager;
        }

        // GET: Catalog
        public async Task<ActionResult> Index(long? catalogId)
        {
            List<Catalog> catalogs;
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _userManager.FindByIdAsync(userId).Result;
            var roles = await _userManager.GetRolesAsync(user);
            var isAdmin = User.IsInRole("Admin");
            if (isAdmin)
            {
                catalogs = _catalogService.GetAllCatalogs(catalogId);
            }
            else
            {
//                var userGroup = roles
//                    .Where(c => c.Type == "userGroup")
//                    .Select(c => c.Value)
//                    .SingleOrDefault();
                catalogs = _catalogService.GetChildCatalogsByUserGroup(catalogId, user.UserGroup.Name);
            }
            var resources = _resourceService.GetAllResourcesForCatalogAndUser(catalogId, user, isAdmin);
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
            Catalog catalog = _db.Catalogs.Find(id);
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
//            if (ModelState.IsValid)
//            {
                var catalog = new Catalog {SuperCatalogId = catalogViewModel.CatalogId, Name = catalogViewModel.Name};
                if (!string.IsNullOrEmpty(catalogViewModel.Groups))
                    catalog.UserGroups = _userGroupService
                        .GetUserGroupsWithNames(catalogViewModel.Groups.Split(',').ToList())
                        .Select(group =>
                        {
                            var userGroupCatalogs = new UserGroupCatalogs();
                            userGroupCatalogs.CatalogId = catalog.Id;
                            userGroupCatalogs.Catalog = catalog;
                            userGroupCatalogs.UserGroupId = group.Id;
                            userGroupCatalogs.UserGroup = group;
                            return userGroupCatalogs;
                        }).ToList();

                _catalogService.SaveCatalog(catalog);
//            }

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
//
//        // POST: Catalog/Delete/5
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public ActionResult DeleteConfirmed(long id)
//        {
//            _catalogService.RemoveCatalog(id);
//            return RedirectToAction("Index");
//        }
//
//        protected override void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                _db.Dispose();
//            }
//            base.Dispose(disposing);
//        }
    }
}
