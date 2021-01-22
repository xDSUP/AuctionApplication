using AutionApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static AutionApp.State;

namespace AutionApp.Services
{
    /// <summary>
    /// Отвечает за обработку лотов со статусами Открыт и ждет открытия
    /// </summary>
    public class LotHandleJob : IJob
    {
        //ApplicationDbContext _dbContext;
        IServiceProvider _services;
        ILogger<LotHandleJob> _logger;

        public LotHandleJob(IServiceProvider services, ILogger<LotHandleJob> logger)
        {
            _services = services;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation($"LotHandleJob начал свою работу");
            using (var scope = _services.CreateScope())
            {
                var _dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var waitedLots = await _dbContext.Lots
                   .Include(l => l.States)
                   .Include(l => l.Bids)
                   .Where(l => l.States.First(s => s.Time == l.States.Max(s => s.Time)).StateId == (int)State.StateLot.WAITED)
                   .ToListAsync();
                _logger.LogInformation($"Найдено ждущих лотов: {waitedLots.Count()}");
                foreach (var lot in waitedLots.Where(l => l.TimeStart < DateTime.Now.AddSeconds(5)))
                    changeLotStatus(_dbContext, lot, State.StateLot.OPENED);


                var openedLots = await _dbContext.Lots
                    .Include(l => l.States)
                    .Include(l => l.Bids).ThenInclude(b=>b.User)
                    .Where(l => l.States.First(s => s.Time == l.States.Max(s => s.Time)).StateId == (int)State.StateLot.OPENED)
                    .ToListAsync();
                _logger.LogInformation($"Найдено открытых лотов: {openedLots.Count()}");

                // пора закрываться
                foreach (var lot in openedLots.Where(l => l.TimeEnd < DateTime.Now.AddSeconds(5)))
                {
                    // если ставок не было
                    if(lot.Bids.Count == 0)
                    {
                        changeLotStatus(_dbContext, lot, StateLot.CLOSED);
                    }
                    else
                    {
                        // последняя ставка, она и победила
                        var maxBid = lot.Bids.First(b => b.Time == lot.Bids.Max(b => b.Time));
                        // добавляем инфу о продаже
                        _logger.LogInformation($"Лот {lot.LotId} продан {maxBid.User.UserName} за {maxBid.Rate} было ставок: {lot.Bids.Count()}");
                        _dbContext.Sells.Add(new Sell { LotId = lot.LotId, UserId = maxBid.UserId });
                        changeLotStatus(_dbContext, lot, StateLot.WAITED_MONEY);
                    }
                }
                await _dbContext.SaveChangesAsync();
            }
        }

        private void changeLotStatus(ApplicationDbContext _dbContext, Lot lot, StateLot newStatus)
        {
            _logger.LogInformation($"{DateTime.Now} Изменен статус лота {lot.LotId} на {State.getText(newStatus)} ");
            _dbContext.StatesLots.Add(new StatesLots { LotId = lot.LotId, Time = DateTime.Now, StateId = (int)newStatus });
        }
    }
}
