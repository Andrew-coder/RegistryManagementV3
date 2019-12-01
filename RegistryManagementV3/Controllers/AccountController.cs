using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RegistryManagementV3.Models;
using RegistryManagementV3.Models.Domain;
using RegistryManagementV3.Services.Notifications;

namespace RegistryManagementV3.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISmsUserNotifier _smsUserNotifier;
        private readonly IEmailUserNotifier _emailUserNotifier;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
            ISmsUserNotifier smsUserNotifier, IEmailUserNotifier emailUserNotifier, ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _smsUserNotifier = smsUserNotifier;
            _emailUserNotifier = emailUserNotifier;
            _logger = logger;
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var result = 
                    await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, false);
                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction("GetTwoFactorAuthentication", new {ReturnUrl = returnUrl });
                }
                if (result.Succeeded)
                {
                    var redirectUrl = string.IsNullOrWhiteSpace(returnUrl) ? "/Home/Index" : returnUrl;
                    return Redirect(redirectUrl);
                }
//                foreach (var error in result.E)
//                {
//                    ModelState.AddModelError(string.Empty, error.Description);
//                }
            }
            return View(model);
        }
        
        // GET: /Login/Get2FA
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetTwoFactorAuthentication(string returnUrl = null)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return RedirectToAction("Login", new {ReturnUrl = returnUrl});
            }

            await NotifyUserWithSms(user);
            var model = new VerifyCodeViewModel {Provider = TokenOptions.DefaultPhoneProvider, ReturnUrl = returnUrl};
            return View("VerifyCode", model);
        }

        // POST: /Login/VerifyTwoFactorAuthentication
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyTwoFactorAuthentication(VerifyCodeViewModel model, string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
 
            if (ModelState.IsValid)
            {
                var result = await _signInManager.TwoFactorSignInAsync(model.Provider, model.Code, false, false);
                if (result.Succeeded)
                {
                    if (returnUrl != null)
                        return Redirect(returnUrl);
 
                    return RedirectToAction("Index", "Home");
                }
 
                if (result.IsLockedOut)
                {
                    ViewBag.ErrorMessage = "You are locked out";
                    return RedirectToAction("Index", "Home");
                }
            }
	
            ViewBag.ErrorMessage = "Token is invalid";
            return View("VerifyCode", model);
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Name, Email = model.Email, AccountStatus = AccountStatus.PendingApproval};
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "User");
                    await _signInManager.SignInAsync(user, isPersistent:false);
                    
                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    return RedirectToAction(nameof(ForgotPasswordConfirmation));
                }
                
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Account", 
                    new { userId = user.Id, code = code }, protocol: Request.Scheme);
                _logger.LogInformation("Generate restore link: {CallbackUrl} for user with id: {UserId}", callbackUrl,
                    user.Id);
                await _emailUserNotifier.NotifyAsync(new EmailNotificationDto
                {
                    Emails = new List<string> {model.Email}, NotificationType = NotificationType.RmRestorePassword,
                    Content = $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>"
                });
                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }

            return View(model);
        }
        
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }
        
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            if (code == null)
            {
                return View("UnexistedEmail");
            }
            var model = new ResetPasswordViewModel { Code = code };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return View("UnexistedEmail");
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            AddErrors(result);
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
        
        // GET: /Account/LogOff
        [HttpGet]
        public async Task<ActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        
        private async Task NotifyUserWithSms(ApplicationUser user)
        {
            var token = await _userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultPhoneProvider);
            _logger.LogInformation("Generate one time password: {Token} for user with id: {UserId}", token, user.Id);
            var notificationDto = new SmsNotificationDto
            {
                PhoneNumbers = new List<string> {user.PhoneNumber},
                NotificationType = NotificationType.RmAuthOtp,
                Content = $"One time password: {token}"
            };
            await _smsUserNotifier.NotifyAsync(notificationDto);
            
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}