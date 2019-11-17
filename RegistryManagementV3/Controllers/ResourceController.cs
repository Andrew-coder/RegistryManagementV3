using System;
using System.IO;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using RegistryManagementV3.Models;
using RegistryManagementV3.Models.Domain;
using RegistryManagementV3.Services;
using RegistryManagementV3.Services.Extensions;

namespace RegistryManagementV3.Controllers
{
    public class ResourceController : Controller
    {
        private readonly IUserService _userService;
        private readonly IResourceService _resourceService;

        public ResourceController(IResourceService resourceService, IUserService userService)
        {
            _resourceService = resourceService;
            _userService = userService;
        }

        // GET: Resource/Create
        public ActionResult Create(long? catalogId)
        {
            var createdResource = new ResourceViewModel {CatalogId = catalogId.GetValueOrDefault()};
            return View(createdResource);
        }

        // POST: Resource/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ResourceViewModel resourceViewModel)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = _userService.GetById(userId);
                
                _resourceService.CreateResourceOnBehalfOfUser(resourceViewModel, user);
                return RedirectToAction("Index", "Catalog");
            }
            catch (Exception ex)
            {
                ViewBag.Message = "ERROR:" + ex.Message.ToString();
            }

            return View(resourceViewModel);
        }
        
        //GET: Resource/MakeEditable
        public ActionResult MakeEditable(long id)
        {
            var resource = _resourceService.MakeEditable(id);
            ViewBag.Readonly = !resource.IsEditable;
            return View("Edit", resource);
        }

        // GET: Resource/Edit/5
        public ActionResult Edit(long id)
        {
            var resource = _resourceService.GetById(id);
            ViewBag.Readonly = !resource.IsEditable;
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
        public ActionResult Delete(long id)
        {
            var resource = _resourceService.GetById(id);
            return View(resource);
        }

        // POST: Resource/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            _resourceService.MarkResourceAsDeleted(id);
            return RedirectToAction("Index", "Catalog");
        }

        public IActionResult DownloadResourceDocument(string fileName)
        {
            var file = Path.Combine(fileName);
            return File(System.IO.File.ReadAllBytes(file), "application/octet-stream", fileName.RemoveTimeStamp());
        }
    }
}
