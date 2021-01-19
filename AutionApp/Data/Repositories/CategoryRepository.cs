using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutionApp.Data.Repositories
{
    public class CategoryRepository
    {
        ApplicationDbContext dbContext;

        public CategoryRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<Category>> GetChilds(Category category)
        {
            var list = new List<Category>();
            await GetChilds(category, list);
            return list;
        }

        private async Task<bool> GetChilds(Category category, List<Category> categories)
        {
            categories.Add(category);
            foreach (var child in category.Childs)
            {
                var c = dbContext.Categories.Include(ch => ch.Childs).First(c => c.CategoryId == child.CategoryId);
                GetChilds(c, categories);
            }
            return true;
        }
    }
}
