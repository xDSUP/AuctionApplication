using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AutionApp;
using AutionApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace AutionApp.Controllers
{
    public class LotsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        IWebHostEnvironment _appEnvironment;
        private readonly ILogger<LotsController> _logger;

        public LotsController(ApplicationDbContext context, UserManager<User> userManager, IWebHostEnvironment appEnvironment, ILogger<LotsController> logger)
        {
            _context = context;
            _userManager = userManager;
            _appEnvironment = appEnvironment;
            _logger = logger;
        }

        // GET: Lots
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Lots.Include(l => l.Category).Include(l => l.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Lots/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lot = await _context.Lots
                .Include(l => l.Category)
                .Include(l => l.User)
                .Include(l => l.Bids).ThenInclude(b=>b.User)
                .Include(l => l.States).ThenInclude(s=>s.State)
                .FirstOrDefaultAsync(m => m.LotId == id);
            if (lot == null)
            {
                return NotFound();
            }

            ViewBag.currentPrice = lot.Bids.Count > 0 ? lot.Bids.Max(b => b.Rate) : lot.StartPrice;

            // инфа о статусах есть всегда, тк он создается при создании лота
            var latestStateTime = lot.States.Max(s => s.Time);
            var latestState = lot.States.Where(s => s.Time == latestStateTime).First();

            ViewBag.latestState = latestState;
            return View(lot);
        }

        // GET: Lots/Create
        [Authorize(Roles = "User")]
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Title");
            return View();
        }

        // POST: Lots/Create
        [Authorize(Roles = "User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LotId,Title,Desc,TimeStart,TimeEnd,StartPrice,Step,CategoryId,UserId")] Lot lot, IFormFile uploadedFile)
        {
            if (ModelState.IsValid)
            {
                lot.UserId = _userManager.GetUserId(User);
                if (uploadedFile != null)
                {
                    // можно грузить только картинки!!!
                    if (!uploadedFile.ContentType.Contains("image"))
                    {
                        ModelState.AddModelError("", "Загружать можно только фотографии");
                        ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", lot.CategoryId);
                        return View(lot);
                    }
                    // путь к папке пользователя
                    //string path = $"/img/{lot.UserId}";
                    //DirectoryInfo directory = new DirectoryInfo(_appEnvironment.WebRootPath + path);
                    //// если такой директории не было, тогда создаем ее
                    //if (!directory.Exists)
                    //    directory.Create();
                    //// сохраняем файл в папку Files в каталоге wwwroot
                    //path += "/" + DateTime.Now.ToUniversalTime().ToString() + uploadedFile.FileName;
                    //using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                    //    await uploadedFile.CopyToAsync(fileStream);
                    using (var stream = new MemoryStream())
                    {
                        await uploadedFile.CopyToAsync(stream);
                        lot.Photo = stream.ToArray();
                    }
                }
                _context.Add(lot);
                await _context.SaveChangesAsync();

                // после добавления, добавим так же статус к новому лоту
                StatesLots statesLots = new StatesLots
                {
                    LotId = lot.LotId,
                    Time = DateTime.Now,
                    StateId = _context.States.Where(s => s.Title==State.getText(State.StateLot.WAITED)).First().StateId
                };
                _logger.Log(LogLevel.Information, $"{statesLots.Time} - Новый статус \"{statesLots.StateId}\" у лота {statesLots.LotId} ");
                _context.StatesLots.Add(statesLots);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", lot.CategoryId);
            return View(lot);
        }

        // GET: Lots/Edit/5
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var lot = await _context.Lots.FindAsync(id);
            if (lot == null)
                return NotFound();

            if (lot.UserId != _userManager.GetUserId(User))
                return RedirectToAction("Details", new { id = id });
            ViewData["Categories"] = new SelectList(_context.Categories, "CategoryId", "Title", lot.CategoryId);
            return View(lot);
        }

        // POST: Lots/Edit/5
        [Authorize(Roles = "User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LotId,Title,Desc,TimeStart,TimeEnd,StartPrice,Step,CategoryId,UserId")] Lot lot)
        {
            if (id != lot.LotId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lot);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LotExists(lot.LotId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", lot.CategoryId);
            ViewData["UserId"] = new SelectList(_context.Set<User>(), "Id", "Id", lot.UserId);
            return View(lot);
        }

        // GET: Lots/Delete/5
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Delete(int? id)
        {
            // TODO: проверка на пользователя
            if (id == null)
            {
                return NotFound();
            }

            var lot = await _context.Lots
                .Include(l => l.Category)
                .Include(l => l.User)
                .FirstOrDefaultAsync(m => m.LotId == id);
            if (lot == null)
            {
                return NotFound();
            }

            return View(lot);
        }

        // POST: Lots/Delete/5
        [Authorize(Roles = "User")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lot = await _context.Lots.FindAsync(id);
            _context.Lots.Remove(lot);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<IActionResult> CreateBid(Bid bid)
        {
            _logger.Log(LogLevel.Information, $"Запрос на создание ставки {bid.Rate} для лота {bid.LotId} от {User.Identity.Name}");
            // берем последнюю сделанную ставку
            var latestBid = _context.Bids.Where(b => b.LotId == bid.LotId).OrderByDescending(b => b.Time).Take(1).FirstOrDefault();
            var lot = await _context.Lots
                .Include(l => l.User)
                .Include(l=> l.Category)
                .Include(l => l.Bids)
                .Include(l => l.States)
                .FirstOrDefaultAsync(m => m.LotId == bid.LotId);

            if (lot == null)
                return NotFound();

            if (lot.TimeStart > DateTime.Now)
                ModelState.AddModelError("", $"Торги ещё не начались");
            else if (lot.TimeEnd < DateTime.Now)
                ModelState.AddModelError("", $"Торги закончились");
            if (latestBid != null && latestBid.Rate > bid.Rate || lot.StartPrice + lot.Step > bid.Rate)
            {
                ModelState.AddModelError("", $"Ваша ставка меньше, чем последняя ставка {latestBid.Rate} по данному лоту");
            }
            var userId = _userManager.GetUserId(User);
            if (userId == lot.UserId)
            {
                ModelState.AddModelError("", $"Нельзя создавать ставки на свои лоты");
            }
            if (ModelState.ErrorCount == 0)
            {
                await _context.Bids.AddAsync(bid);
                // Todo: Обработать исключение
                await _context.SaveChangesAsync();
                _logger.Log(LogLevel.Information, $"Ставка {bid.Rate} для лота {bid.LotId} от {User.Identity.Name} создана");
                return RedirectToAction("Details", new { id = bid.LotId });
            }
            else
            {
                return View("Details", lot);
            }
        }

        private bool LotExists(int id)
        {
            return _context.Lots.Any(e => e.LotId == id);
        }
    }
}
