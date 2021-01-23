using AutionApp.Data;
using AutionApp.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutionApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<AccountController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(ApplicationDbContext dbContext, ILogger<AccountController> logger, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _dbContext = dbContext;
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: AccountController (если пользователь авторизован, покажем его личный кабинет)
        public ActionResult Index()
        {
            // если пользователь не авторизован
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            // пользователь авторизован, покажем его личный кабинет

            return View();
        }

        // GET: AccountController/Details/id
        public ActionResult Details(string id)
        {
            // если айдишка пустая
            if (String.IsNullOrEmpty(id))
            {
                // если пользователь не авторизован
                if (!User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("Index", "Home");
                }
                // пользователь авторизован, покажем его личный кабинет
                return RedirectToAction("Index");
            }
            return View();
        }

        // GET: AccountController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AccountController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AccountController/Registrer
        [AllowAnonymous]
        public ActionResult Regiter(string returnUrl = null)
        {
            returnUrl = !String.IsNullOrEmpty(returnUrl) ? returnUrl : "~/";
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        // POST: AccountController/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<ActionResult> Regiter(RegisterViewModel model, string returnUrl = null)
        {
            returnUrl = !String.IsNullOrEmpty(returnUrl) ? returnUrl : "~/";

            var user = new User { Email = model.Email, UserName = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                _logger.LogInformation($"Зарегистрировался новый пользователь {model.Email}");
                _userManager.AddToRoleAsync(user, "User");
                await _signInManager.SignInAsync(user, isPersistent: false);
                return LocalRedirect(returnUrl);
            }
            else
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
            }

            ViewBag.returnUrl = returnUrl;
            return View(model);
        }

        // GET: AccountController/Login
        [AllowAnonymous]
        public async Task<ActionResult> Login(string returnUrl = null)
        {
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            returnUrl = !String.IsNullOrEmpty(returnUrl) ? returnUrl : "~/";
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        // POST: AccountController/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            returnUrl = !String.IsNullOrEmpty(returnUrl) ? returnUrl : "~/";
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation($"User {User.Identity.Name} logged in.");
                    return LocalRedirect(returnUrl);
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("Пользовательский аккаунт заблокирован");
                    ModelState.AddModelError(string.Empty, "Пользовательский аккаунт заблокирован");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Не удалось авторизоваться");
                }
            }
            ViewBag.returnUrl = returnUrl;
            return View(model);
        }

        // POST: AccountController/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Logout()
        {
            _logger.LogInformation($"User {User.Identity.Name} logged out.");
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Index), "Home");
        }
    }
}
