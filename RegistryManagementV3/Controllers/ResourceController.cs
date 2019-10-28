using System;
using Microsoft.AspNetCore.Mvc;
using RegistryManagementV3.Controllers.Attributes;
using RegistryManagementV3.Models;
using RegistryManagementV3.Models.Domain;
using RegistryManagementV3.Models.Repository;
using RegistryManagementV3.Services;

namespace RegistryManagementV3.Controllers
{
    //[ClaimsAuthorize(AccountStatus = AccountStatus.Approved)]
    public class ResourceController : Controller
    {
        private const int DefaultSecurityLevel = 5;
        private readonly SecurityDbContext _db;
        private readonly IResourceService _resourceService;
        private readonly ICatalogService _catalogService;

        public ResourceController(SecurityDbContext db, IResourceService resourceService, ICatalogService catalogService)
        {
            _db = db;
            _resourceService = resourceService;
            _catalogService = catalogService;
        }

        // GET: Resource/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(400);
            }
            var resource = _db.Resources.Find(id);
            if (resource == null)
            {
                return new StatusCodeResult(404);
            }
            return View(resource);
        }

        // GET: Resource/Create
        public ActionResult Create(long? catalogId)
        {
            var catalog = _catalogService.GetById(catalogId.GetValueOrDefault());
            var securityLevel = DefaultSecurityLevel;
            if (catalog != null)
            {
                securityLevel = catalog.SecurityLevel;
            }

            var createdResource = new ResourceViewModel {SecurityLevel = securityLevel};
            var tuple = new Tuple<ResourceViewModel, long?>(createdResource, catalogId);
            return View(tuple);
        }

        // POST: Resource/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ResourceViewModel resourceViewModel)
        {
            long? catalogId = 0;
//            if (ModelState.IsValid)
//            {
                try
                {    
                    catalogId = resourceViewModel.CatalogId ?? default(int);
                    _resourceService.CreateResource(resourceViewModel, catalogId ?? 0);
                    return RedirectToAction("Index", "Catalog");  
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                } 
//            }
            var tuple = new Tuple<ResourceViewModel, long?>(resourceViewModel, catalogId);
            return View(tuple);
        }

        // GET: Resource/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(400);
            }
            var resource = _db.Resources.Find(id);
            if (resource == null)
            {
                return new StatusCodeResult(404);
            }
            var tuple = new Tuple<Resource, long?>(resource, resource.CatalogId);
            return View(tuple);
        }

        // POST: Resource/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UpdateResourceViewModel resourceViewModel)
        {
            var resource = _resourceService.GetById(resourceViewModel.Id.GetValueOrDefault(-1));
            if (resource == null)
            {
                return new StatusCodeResult(400);
            }
            if (!User.IsInRole("Admin"))
            {
                resource.ResourceStatus = ResourceStatus.PendingForEditApprove;
            }
            if (ModelState.IsValid)
            {
                _resourceService.UpdateResource(resourceViewModel, resource);
            }
            var tuple = new Tuple<Resource, long?>(resource, resource.CatalogId);
            return View(tuple);
        }

        // GET: Resource/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(400);
            }
            Resource resource = _db.Resources.Find(id);
            if (resource == null)
            {
                return new StatusCodeResult(404);
            }
            return View(resource);
        }

        // POST: Resource/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var resource = _db.Resources.Find(id);
            _db.Resources.Remove(resource);
            _db.SaveChanges();
            return RedirectToAction("Index", "Catalog");
        }
//
//        protected override void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                _db.Dispose();
//            }
//            base.Dispose(disposing);
//        }

        public ActionResult ViewResourceDocument(string fileName)
        {
            System.Diagnostics.Process.Start(fileName);
            return RedirectToAction("Index", "Catalog");
        }
    }
}
