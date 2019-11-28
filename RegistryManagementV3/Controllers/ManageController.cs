using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RegistryManagementV3.Models;
using RegistryManagementV3.Models.Domain;
using RegistryManagementV3.Services;
using RegistryManagementV3.Services.Notifications;

namespace RegistryManagementV3.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserService _userService;
        private readonly IUserNotifier _userNotifier;
        
        public ManageController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IUserService userService, IUserNotifier userNotifier)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userService = userService;
            _userNotifier = userNotifier;
        }

        //
        // GET: /Manage/Index
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.SetTwoFactorSuccess ? "Your two-factor authentication provider has been set."
                : message == ManageMessageId.Error ? "An error has occurred."
                : message == ManageMessageId.AddPhoneSuccess ? "Your phone number was added."
                : message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed."
                : "";

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _userService.GetById(userId);
            var model = new IndexViewModel
            {
                HasPassword = user.PasswordHash != null,
                PhoneNumber = user.PhoneNumber,
                TwoFactor = user.TwoFactorEnabled,
                Logins = await _userManager.GetLoginsAsync(user),
                BrowserRemembered = await _signInManager.IsTwoFactorClientRememberedAsync(user)
            };
            return View(model);
        }

        //
        // POST: /Manage/RemoveLogin
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
//        {
//            ManageMessageId? message;
//            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
//            var result = await _userManager.RemoveLoginAsync(userId, new UserLoginInfo(loginProvider, providerKey));
//            if (result.Succeeded)
//            {
//                var user = await _userManager.FindByIdAsync(userId);
//                if (user != null)
//                {
//                    await _signInManager.SignInAsync(user, isPersistent: false);
//                }
//                message = ManageMessageId.RemoveLoginSuccess;
//            }
//            else
//            {
//                message = ManageMessageId.Error;
//            }
//            return RedirectToAction("ManageLogins", new { Message = message });
//        }
//
//        //
//        // GET: /Manage/AddPhoneNumber
        public ActionResult AddPhoneNumber()
        {
            return View();
        }

//        //
//        // POST: /Manage/AddPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Generate the token and send it
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _userService.GetById(userId);
            var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, model.Number);
            if (_userNotifier != null)
            {
                var userNotificationDto = new UserNotificationDto()
                {
                    PhoneNumbers = new List<string> {model.Number},
                    Content = "Your security code is: " + code
                };
                await _userNotifier.NotifyAsync(userNotificationDto);
            }
            return RedirectToAction("VerifyPhoneNumber", new { PhoneNumber = model.Number });
        }
//
//        //
//        // POST: /Manage/EnableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EnableTwoFactorAuthentication()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _userService.GetById(userId);
            if (user != null)
            {
                _userService.SetTwoFactorEnabled(user, true);
                await _signInManager.SignInAsync(user, isPersistent: false);
            }
            return RedirectToAction("Index", "Manage");
        }
//
//        //
//        // POST: /Manage/DisableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DisableTwoFactorAuthentication()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _userService.GetById(userId);
            await _userManager.SetTwoFactorEnabledAsync(user, false);
            if (user != null)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
            }
            return RedirectToAction("Index", "Manage");
        }
//
//        //
//        // GET: /Manage/VerifyPhoneNumber
        public async Task<ActionResult> VerifyPhoneNumber(string phoneNumber)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _userService.GetById(userId);
            var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, phoneNumber);
            // Send an SMS through the SMS provider to verify the phone number
            return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
        }
//
//        //
//        // POST: /Manage/VerifyPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _userService.GetById(userId);
            var result = await _userManager.ChangePhoneNumberAsync(user, model.PhoneNumber, model.Code);
            if (result.Succeeded)
            {
                if (user != null)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.AddPhoneSuccess });
            }
            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "Failed to verify phone");
            return View(model);
        }
//
//        //
//        // POST: /Manage/RemovePhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemovePhoneNumber()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _userService.GetById(userId);
            var result = await _userManager.SetPhoneNumberAsync(user, null);
            if (!result.Succeeded)
            {
                return RedirectToAction("Index", new { Message = ManageMessageId.Error });
            }
            if (user != null)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
            }
            return RedirectToAction("Index", new { Message = ManageMessageId.RemovePhoneSuccess });
        }
//
//        //
//        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }
//
//        //
//        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                if (user != null)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }
//
//        //
//        // GET: /Manage/SetPassword
        public ActionResult SetPassword()
        {
            return View();
        }
//
//        //
//        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = _userService.GetById(userId);
                var result = await _userManager.AddPasswordAsync(user, model.NewPassword);
                if (result.Succeeded)
                {
                    if (user != null)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                    }
                    return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
//
//        #region Helpers
//        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";
//
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
//
//        private bool HasPassword()
//        {
//            var user = _userManager.FindById(User.Identity.GetUserId());
//            if (user != null)
//            {
//                return user.PasswordHash != null;
//            }
//            return false;
//        }
//
//        private bool HasPhoneNumber()
//        {
//            var user = _userManager.FindById(User.Identity.GetUserId());
//            if (user != null)
//            {
//                return user.PhoneNumber != null;
//            }
//            return false;
//        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }


    }
}