using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Wagebat.Data;
using Wagebat.Models;

namespace Wagebat.Controllers
{
    public class SubscriptionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public SubscriptionsController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Subscriptions
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Subscriptions.Include(s => s.Confirmer).Include(s => s.Package).Include(s => s.Status).Include(s => s.User);
            return View(await applicationDbContext.ToListAsync());
        }
        
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ConfirmationsIndex()
        {
            var subscriptions = await _context.Subscriptions
                .Include(s => s.Confirmer)
                .Include(s => s.Package)
                .Include(s => s.Status)
                .Include(s => s.User)
                .Where(s => s.Confirmer == null).ToListAsync();

            return View(subscriptions);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<JsonResult> Confirm (int? id)
        {
            if (id == null)
                return Json(false);

            var subscription = await _context.Subscriptions
                .Include(s => s.Confirmer)
                .Include(s => s.Package)
                .Include(s => s.Status)
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (subscription == null)
                return Json(false);

            subscription.Confirmer = await _userManager.FindByNameAsync(User.Identity.Name);
            _context.Subscriptions.Update(subscription);
            await _context.SaveChangesAsync();

            return Json(true);
        }

        // GET: Subscriptions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subscription = await _context.Subscriptions
                .Include(s => s.Confirmer)
                .Include(s => s.Package)
                .Include(s => s.Status)
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subscription == null)
            {
                return NotFound();
            }

            return View(subscription);
        }

        // GET: Subscriptions/Create
        public IActionResult Create(int id)
        {
            ViewData["Package"] = _context.Packages.Find(id);
            return View();
        }

        // POST: Subscriptions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Package package)
        {
            if (!ModelState.IsValid)
               return RedirectToAction("Index", "Packages");
            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
            var subscription = new Subscription
            {
                UserId = currentUser.Id,
                Date = DateTime.Now,
                PackageId = package.Id,
                StatusId = 1,
            };

            _context.Add(subscription);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Subscriptions/Edit/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subscription = await _context.Subscriptions.FindAsync(id);
            if (subscription == null)
            {
                return NotFound();
            }
            ViewData["ConfirmerId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", subscription.ConfirmerId);
            ViewData["PackageId"] = new SelectList(_context.Packages, "Id", "Id", subscription.PackageId);
            ViewData["StatusId"] = new SelectList(_context.Statuses, "Id", "Id", subscription.StatusId);
            ViewData["UserId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", subscription.UserId);
            return View(subscription);
        }

        // POST: Subscriptions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,PackageId,StatusId,Date,ConfirmerId")] Subscription subscription)
        {
            if (id != subscription.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(subscription);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubscriptionExists(subscription.Id))
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
            ViewData["ConfirmerId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", subscription.ConfirmerId);
            ViewData["PackageId"] = new SelectList(_context.Packages, "Id", "Id", subscription.PackageId);
            ViewData["StatusId"] = new SelectList(_context.Statuses, "Id", "Id", subscription.StatusId);
            ViewData["UserId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", subscription.UserId);
            return View(subscription);
        }

        // GET: Subscriptions/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subscription = await _context.Subscriptions
                .Include(s => s.Confirmer)
                .Include(s => s.Package)
                .Include(s => s.Status)
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subscription == null)
            {
                return NotFound();
            }

            return View(subscription);
        }

        // POST: Subscriptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subscription = await _context.Subscriptions.FindAsync(id);
            _context.Subscriptions.Remove(subscription);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SubscriptionExists(int id)
        {
            return _context.Subscriptions.Any(e => e.Id == id);
        }
    }
}
