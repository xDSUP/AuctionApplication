using AutionApp.Data;
using AutionApp.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
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
        public async Task<ActionResult> Index()
        {
            // если пользователь не авторизован
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            // пользователь авторизован, покажем его личный кабинет
            var model = new IndexViewModel();
            model.User = _dbContext.Users
                .Include(u=>u.Lots)
                .Include(u=>u.Sells).ThenInclude(s=>s.Lot)
                .Include(u=>u.Bids).ThenInclude(b => b.Lot)
                .First(u => u.Id == _userManager.GetUserId(User));
            model.Feedbacks = await _dbContext.Feedbacks.Where(f => f.UserId == model.User.Id)
                .Include(f=>f.User)
                .Include(f => f.Author)
                .ToListAsync();

            return View(model);
        }

        // GET: AccountController/Details/id
        public async Task<ActionResult> Details(string id)
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

            var model = new IndexViewModel();
            model.User = _dbContext.Users
                .Include(u => u.Lots)
                .Include(u => u.Sells).ThenInclude(s => s.Lot)
                .Include(u => u.Bids).ThenInclude(b => b.Lot)
                .First(u => u.Id == id);
            model.Feedbacks = await _dbContext.Feedbacks.Where(f => f.UserId == model.User.Id)
                .Include(f => f.User)
                .Include(f => f.Author)
                .ToListAsync();

            return View(model);
        }


        // GET: AccountController/Edit/5
        [Authorize(Roles = "User")]
        public ActionResult Edit(string id)
        {
            
            return View();
        }

        // POST: AccountController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(User user)
        {
            try
            {
                if (user.Id == _userManager.GetUserId(User))
                {
                    _dbContext.Users.Update(user);
                    await _dbContext.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AccountController/Registrer
        [AllowAnonymous]
        public ActionResult Register(string returnUrl = null)
        {
            returnUrl = !String.IsNullOrEmpty(returnUrl) ? returnUrl : "~/";
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        // POST: AccountController/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<ActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            returnUrl = !String.IsNullOrEmpty(returnUrl) ? returnUrl : "~/";

            var user = new User { Email = model.Email, UserName = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                _logger.LogInformation($"Зарегистрировался новый пользователь {model.Email}");
                await _userManager.AddToRoleAsync(user, "User");
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

        [HttpPost]
        [Authorize(Roles = "User")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateFeedback(Feedback model)
        {
            if (ModelState.IsValid)
            {
                model.AuthorId = _userManager.GetUserId(User);
                model.Time = DateTime.Now;
                var result = await _dbContext.Feedbacks.AddAsync(model);
                await _dbContext.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Details), new { id=model.UserId });
        }
    }
}
