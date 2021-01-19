using AutionApp.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutionApp.Data.Repositories
{
    public class LotRepository
    {
        private readonly ApplicationDbContext dbContext;
        public LotRepository(ApplicationDbContext context)
        {
            dbContext = context;
        }

        /// <summary>
        /// Получение списка лотов для выбранных категорий
        /// </summary>
        /// <param name="categories">список всех выбранных категорий</param>
        /// <returns></returns>
        public IEnumerable<Lot> GetLotsByCategories(List<int> categories)
        {
            return dbContext.Lots.Include(l => l.User).Include(l=>l.Category).Include(l=>l.Bids).Where(l=>categories.Contains(l.CategoryId)).ToList();
        }
    }
}
