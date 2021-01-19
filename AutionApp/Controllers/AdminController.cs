using AutionApp.Data;
using AutionApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutionApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<UserRole> _roleManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<AdminController> _logger;

        public AdminController(UserManager<User> userManager, RoleManager<UserRole> roleManager, ApplicationDbContext context, ILogger<AdminController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region Users
        public async Task<IActionResult> Users()
        {
            List<UserViewModel> viewModel = new List<UserViewModel>();
            foreach (var user in _userManager.Users.ToList())
            {
                var roles = await _userManager.GetRolesAsync(user);
                viewModel.Add(new UserViewModel { User = user, Roles = roles.ToList() });
            }
            return View(viewModel);
        }

        public IActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User
                {
                    Email = model.Email,
                    UserName = model.Email
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                    return RedirectToAction("Users");
                else
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        public async Task<IActionResult> EditUser(String id)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();
            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = _roleManager.Roles.ToList();
            EditUserViewModel model = new EditUserViewModel { Id = user.Id, Email = user.Email, UserRoles=userRoles, AllRoles=allRoles};
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(string userId, List<string> roles)
        {
            User user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                // роли, которые сохранены в БД
                var userRoles = await _userManager.GetRolesAsync(user);
                // все роли
                var allRoles = _roleManager.Roles.ToList();
                // добавленные роли
                var addedRoles = roles.Except(userRoles);
                // удаленные роли
                var removedRoles = userRoles.Except(roles);
                await _userManager.AddToRolesAsync(user, addedRoles);
                await _userManager.RemoveFromRolesAsync(user, removedRoles);
                return RedirectToAction("Users");
            }
            return NotFound();

        }

        [HttpPost]
        public async Task<ActionResult> DeleteUser(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);
            }
            return RedirectToAction("Users");
        }
        #endregion
   
        #region Roles
        public async Task<IActionResult> Roles()
        {
            return View(_roleManager.Roles.ToList());
        }

        public async Task<IActionResult> CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                IdentityResult result = await _roleManager.CreateAsync(new UserRole(name));
                if (result.Succeeded)
                    return RedirectToAction("Roles");
                else
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(name);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role != null)
            {
                IdentityResult result = await _roleManager.DeleteAsync(role);
            }
            return RedirectToAction("Roles");
        }

        private static object RedirectToActionResult()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Categories
        public async Task<IActionResult> Categories()
        {
            return View(_dbContext.Categories.ToList());
        }

        public async Task<IActionResult> CreateCategory()
        {
            ViewBag.AllCategories = new SelectList(_dbContext.Categories.ToList(), "CategoryId", "Title");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(Category category)
        {
            // TODO: Проставлять IsGroup внутри, когда подвязывается хотя бы одна другая категория, а не через пользователя
            if (category.IsGroup)
                category.Parent = _dbContext.Categories.FirstOrDefault(cat => cat.CategoryId == category.ParentId);
            else
                category.ParentId = null;
            
            var result = await _dbContext.Categories.AddAsync(category);
            if(result.State == Microsoft.EntityFrameworkCore.EntityState.Added)
            {
                await _dbContext.SaveChangesAsync();
                return RedirectToAction("Categories");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Ошибочка");
            }
            return View(category);
        }

        public async Task<IActionResult> EditCategory(int id)
        {
            Category category = _dbContext.Categories.FirstOrDefault(c => c.CategoryId == id);
            if (category == null)
                return NotFound();
            ViewBag.AllCategories = new SelectList(_dbContext.Categories.ToList(), "CategoryId", "Title");
            return View(category);
        }
        
        [HttpPost]
        public async Task<IActionResult> EditCategory(Category category)
        {
            if (category.IsGroup)
                category.Parent = _dbContext.Categories.FirstOrDefault(cat => cat.CategoryId == category.ParentId);
            else
                category.ParentId = null;
            
            var result = _dbContext.Categories.Update(category);
            if (result.State == Microsoft.EntityFrameworkCore.EntityState.Modified)
            {
                await _dbContext.SaveChangesAsync();
                return RedirectToAction("Categories");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Ошибочка");
            }
            ViewBag.AllCategories = new SelectList(_dbContext.Categories.ToList(), "CategoryId", "Title");
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            Category category = _dbContext.Categories.FirstOrDefault(c => c.CategoryId == id);
            if (category == null)
                return NotFound();
            _dbContext.Categories.Remove(category);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Categories");
        }
        #endregion

        #region Lots
        #endregion
    }
}
