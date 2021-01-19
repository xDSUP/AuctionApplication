using AutionApp.Data;
using AutionApp.Data.Repositories;
using AutionApp.Models;
using AutionApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace AutionApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly CategoryRepository categoryRepository;

        

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _dbContext = context;
            categoryRepository = new CategoryRepository(_dbContext);
        }


        public async Task<IActionResult> Index(int idCategory, int page)
        {
            var category = _dbContext.Categories.Include(c=>c.Childs).FirstOrDefault(cat => cat.CategoryId == idCategory);
            _logger.Log(LogLevel.Information, $"CatId={idCategory}, page={page}, catTitle={category?.Title}, catChildCount={category?.Childs.ToList().Count}");
            if (category == null)
                category = _dbContext.Categories.Include(c => c.Childs).FirstOrDefault(cat => cat.CategoryId == Utils.ID_START_CATEGORY);

            // находим все подкатегории этой категории
            var allCategories = await categoryRepository.GetChilds(category);

            // TODO: добавить лоты на главную
            var lots = await _dbContext.Lots
                .Include(l => l.User)
                .Include(l => l.Category)
                .Include(l => l.Bids)
                .Where(l => allCategories.Contains(l.Category) && l.TimeEnd>DateTime.Now)
                .ToListAsync();
            IndexAucViewModel model = new IndexAucViewModel {
                Categories = category.Childs.ToList(),
                Lots = lots
            };
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
