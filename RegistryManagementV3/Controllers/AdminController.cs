using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RegistryManagementV3.Models;
using RegistryManagementV3.Models.Domain;
using RegistryManagementV3.Services;

namespace RegistryManagementV3.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICatalogService _catalogService;
        private readonly IResourceService _resourceService;

        public AdminController(UserManager<ApplicationUser> userManager, ICatalogService catalogService, IResourceService resourceService)
        {
            _userManager = userManager;
            _catalogService = catalogService;
            _resourceService = resourceService;
        }

        public ActionResult UsersManagement()
        {
            return View(_userManager.Users.ToList());
        }

        // GET: UserManagement
        public ActionResult ResourceManagement(long? catalogId)
        {
            var catalogs = _catalogService.GetAllCatalogs(catalogId);
            var resources = _resourceService.GetAllResources(catalogId);

            var tuple = new Tuple<IList<Catalog>, IList<Resource>, long?>(catalogs, resources, catalogId);
            return View("~/Views/Catalog/Index.cshtml", tuple);
        }

        public ActionResult ApproveResource(long id)
        {
            _resourceService.ApproveResource(id);
            return RedirectToAction("ResourceManagement");
        }
    }
}