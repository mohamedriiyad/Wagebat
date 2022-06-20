using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Wagebat.Data;
using Wagebat.Helpers;
using Wagebat.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Wagebat.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CommentsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Comments
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Comments.Include(c => c.Transaction).Include(c => c.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Comments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();


            var comment = await _context.Comments
                .Include(c => c.Transaction)
                .Include(c => c.User)
                .Include(c => c.CommentAttachments)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comment == null)
                return NotFound();

            return View(comment);
        }

        // GET: Comments/Create
        public IActionResult Create()
        {
            ViewData["TransactionId"] = new SelectList(_context.Transactions, "Id", "Id");
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<JsonResult> Create(Comment comment)
        {
            if (!ModelState.IsValid)
                return Json(false);
            if (comment.Body == null)
                return Json(false);
            var files = Request.Form.Files.ToList();
            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);

            comment.TransactionId = comment.Id;
            comment.Date = DateTime.Now;
            comment.UserId = currentUser.Id;
            comment.Id = 0;

            var pathToSave = Path.Combine("images", "comments");
            List<string> attatchments;
            try
            {
                attatchments = await FileHelper.UploadAll(files, pathToSave);
            }
            catch (Exception ex)
            {
                return Json(false);
            }
            comment.Body = WebUtility.HtmlEncode(comment.Body);
            foreach (var attatchment in attatchments)
            {
                comment.CommentAttachments.Add(new CommentAttachment { Path = attatchment });
            }
            _context.Add(comment);
            await _context.SaveChangesAsync();
            return Json(true);
        }

        // GET: Comments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            ViewData["TransactionId"] = new SelectList(_context.Transactions, "Id", "Id", comment.TransactionId);
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", comment.UserId);
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,TransactionId,Body,Date")] Comment comment)
        {
            if (id != comment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(comment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(comment.Id))
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
            ViewData["TransactionId"] = new SelectList(_context.Transactions, "Id", "Id", comment.TransactionId);
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", comment.UserId);
            return View(comment);
        }

        // GET: Comments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .Include(c => c.Transaction)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.Id == id);
        }
    }
}
