using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RegistryManagementV3.Models;
using RegistryManagementV3.Models.Domain;
using RegistryManagementV3.Models.Repository;
using RegistryManagementV3.Services;

namespace RegistryManagementV3.Controllers
{
   [Authorize(Roles = "Admin")]
    public class UserController : Controller 
    {
        private readonly SecurityDbContext _dbContext; 
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserGroupService _userGroupService;

        public UserController(SecurityDbContext dbContext, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IUserGroupService userGroupService)
        {
            _dbContext = dbContext;
            _signInManager = signInManager;
            _userManager = userManager;
            _userGroupService = userGroupService;
        }

        // GET: User
        public ActionResult Index()
        {
            var roles = _dbContext.Roles.Where(r => r.Name == UserRole.User.ToString());
            var users = new List<ApplicationUser>();
            if (roles.Any())
            {
                var roleId = roles.First().Id;
                users = (from user in _dbContext.Users
                    //where user.Roles.Any(r => r.RoleId == roleId)
                    select user).ToList();
            }
            return View(users);
        }

        // GET: User/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new StatusCodeResult(400);
            }
            var applicationUser = await _userManager.FindByIdAsync(id);
            if (applicationUser == null)
            {
                return new StatusCodeResult(404);
            }
            return View(applicationUser);
        }

        // GET: User/Create
//        public ActionResult Create()
//        {
//            return View();
//        }

        // GET: User/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new StatusCodeResult(400);
            }
            var applicationUser = await _userManager.FindByIdAsync(id);
            var userGroups = _userGroupService.GetAllUserGroups();
            if (applicationUser == null)
            {
                return new StatusCodeResult(404);
            }

            var roles = _dbContext.Roles.ToList();
            var userViewModel = new ApplicationUserViewModel()
            {
                Id = applicationUser.Id,
                Email = applicationUser.Email,
                PhoneNumber = applicationUser.PhoneNumber,
                AccessFailedCount = applicationUser.AccessFailedCount,
                AccountStatus = applicationUser.AccountStatus.ToString(),
                UserName = applicationUser.UserName,
                UserGroup = applicationUser.UserGroup?.Name
            };
            var userInfo = new Tuple<ApplicationUserViewModel, List<IdentityRole>>(userViewModel, roles);
            return View(userInfo);
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind("Id,AccountStatus,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName, UserGroup")] ApplicationUserViewModel applicationUser)
        {
            var userGroup = _userGroupService.GetUserGroupsWithNames(new Collection<string>() { applicationUser.UserGroup});
               if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(applicationUser.Id);
                user.UserGroup = userGroup.First();
                user.Email = applicationUser.Email;
                user.UserName = applicationUser.UserName;
                await _userManager.UpdateAsync(user);
                return RedirectToAction("Index");
            }
            var userGroups = _userGroupService.GetAllUserGroups();
            var userInfo = new Tuple<ApplicationUserViewModel, List<UserGroup>>(applicationUser, userGroups);
            return View(userInfo);
        }

        // GET: User/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new StatusCodeResult(400);
            }
            var applicationUser = await _userManager.FindByIdAsync(id);
            if (applicationUser == null)
            {
                return new StatusCodeResult(404);
            }
            return View(applicationUser);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            var applicationUser = await _userManager.FindByIdAsync(id);
            //_userManager.Remove(applicationUser);
            await _userManager.RemoveFromRoleAsync(applicationUser, UserRole.User.ToString());
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Approve(string id)
        {
            var applicationUser = await _userManager.FindByIdAsync(id);
            applicationUser.AccountStatus = AccountStatus.Approved;
            await _userManager.UpdateAsync(applicationUser);
            return RedirectToAction("Index");
        }
    }
}
