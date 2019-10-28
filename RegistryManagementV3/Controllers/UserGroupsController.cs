using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegistryManagementV3.Models.Domain;
using RegistryManagementV3.Services;

namespace RegistryManagementV3.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserGroupsController : Controller
    {
        private readonly IUserGroupService _userGroupService;

        public UserGroupsController(IUserGroupService userGroupService)
        {
            _userGroupService = userGroupService;
        }

        // GET: UserGroups
        public ActionResult Index()
        {
            return View(_userGroupService.GetAllUserGroups());
        }

        

        // GET: UserGroups/Create
        public ActionResult Create()
        {
            return View(new UserGroup{ SecurityLevel = 5});
        }

        // POST: UserGroups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind("Id,Name,SecurityLevel")] UserGroup userGroup)
        {
            if (ModelState.IsValid)
            {
                _userGroupService.CreateUserGroup(userGroup);
                return RedirectToAction("Index");
            }

            return View(userGroup);
        }

        // GET: UserGroups/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(400);
            }

            var userGroup = _userGroupService.GetUserGroupById(id.Value);
            if (userGroup == null)
            {
                return new StatusCodeResult(404);
            }
            return View(userGroup);
        }

        // POST: UserGroups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind("Id,Name,SecurityLevel")] UserGroup userGroup)
        {
            if (ModelState.IsValid)
            {
                _userGroupService.UpdateUserGroup(userGroup);
                return RedirectToAction("Index");
            }
            return View(userGroup);
        }

        public ActionResult Delete(long id)
        {
            _userGroupService.DeleteUserGroup(id);
            return RedirectToAction("Index");
        }
    }
}
