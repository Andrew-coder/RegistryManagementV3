using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RegistryManagementV3.Models;
using RegistryManagementV3.Models.Domain;
using RegistryManagementV3.Models.Repository;
using RegistryManagementV3.Services;

namespace RegistryManagementV3.Controllers
{
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

            var createdResource = new ResourceViewModel {SecurityLevel = securityLevel, CatalogId = catalogId.GetValueOrDefault()};
            return View(createdResource);
        }

        // POST: Resource/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ResourceViewModel resourceViewModel)
        {
            var catalogId = resourceViewModel.CatalogId ?? 0;
            try
            {
                _resourceService.CreateResource(resourceViewModel, catalogId);
                return RedirectToAction("Index", "Catalog");
            }
            catch (Exception ex)
            {
                ViewBag.Message = "ERROR:" + ex.Message.ToString();
            }

            return View(resourceViewModel);
        }

        // GET: Resource/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(400);
            }

            var resource = _db.Resources
                .Include(s => s.Catalog)
                .Include(s => s.TagResources)
                .ThenInclude(tr => tr.Tag)
                .FirstOrDefault();
            
            if (resource == null)
            {
                return new StatusCodeResult(404);
            }
            return View(resource);
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
            return View(resource);
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

        public IActionResult DownloadResourceDocument(string fileName)
        {
            var file = Path.Combine(fileName);
            return File(System.IO.File.ReadAllBytes(file), "application/octet-stream", fileName);
        }
    }
}
