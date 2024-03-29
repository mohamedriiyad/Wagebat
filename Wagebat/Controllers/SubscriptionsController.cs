﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
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
        public async Task<IActionResult> Index()
        {
            var subscriptions = await _context.Subscriptions
                .Include(s => s.Confirmer)
                .Include(s => s.Package)
                .Include(s => s.Status)
                .Include(s => s.User)
                .Where(s => s.Confirmer != null)
                .ToListAsync();

            return View(subscriptions);
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

        public async Task<IActionResult> InstructorsConfirmationsIndex()
        {
            var instructors = await _context.ApplicationUsers
                .Include(a => a.Courses)
                .Where(a => a.Courses.Count == 0)
                .Where(a => a.EmailConfirmed == false)
                .ToListAsync();

            return View(instructors);
        }
        public async Task<IActionResult> AllInstructorsIndex()
        {
            var instructors = await _context.ApplicationUsers
                .Include(a => a.Courses)
                .Where(a => a.Courses.Count >= 1)
                .ToListAsync();

            return View(instructors);
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
            subscription.StatusId = 2;
            _context.Subscriptions.Update(subscription);
            await _context.SaveChangesAsync();

            return Json(true);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<JsonResult> InstructorsConfirm(string id)
        {
            if (id == null)
                return Json(false);

            var instructor = await _context.ApplicationUsers
                .Include(a => a.Courses)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (instructor == null)
                return Json(false);

            instructor.EmailConfirmed = true;
            _context.ApplicationUsers.Update(instructor);
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

        [Authorize]
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
            try
            {
                SendSubscriptionEmail(currentUser.Email);
            }
            catch (Exception ex)
            {}
            
            _context.Add(subscription);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public void SendSubscriptionEmail(string email)
        {
            var body = "نحيطكم علما بأن الضحك مستمر ";
            string from = "trepletech@outlook.com";
            string password = "M_o0123456";
            MailMessage msg = new MailMessage
            {
                Subject = "Video Conference",
                Body = body,
                From = new MailAddress(from)
            };
            msg.To.Add(new MailAddress(email));

            SmtpClient smtp = new SmtpClient("smtp-mail.outlook.com");
            smtp.Port = 587;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.UseDefaultCredentials = false;
            smtp.EnableSsl = true;
            
            NetworkCredential nc = new NetworkCredential(from, password);
            smtp.Credentials = nc;
            smtp.Send(msg);
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
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<JsonResult> DeleteConfirmed(int id)
        {
            {
            var subscription = await _context.Subscriptions.FindAsync(id);
                if (subscription == null)
                    return Json(false);

                subscription.Confirmer = null;
                _context.Subscriptions.Update(subscription);
                try
                {
                    await _context.SaveChangesAsync();
                }catch(Exception ex)
                {

                }
                return Json(true);
            }
        }

        private bool SubscriptionExists(int id)
        {
            return _context.Subscriptions.Any(e => e.Id == id);
        }
    }
}
