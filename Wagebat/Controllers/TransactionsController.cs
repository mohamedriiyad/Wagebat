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

namespace Wagebat.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TransactionsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Transactions
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Transactions.Include(t => t.Acceptor).Include(t => t.Question).Include(t => t.Status);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Transactions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .Include(t => t.Acceptor)
                .Include(t => t.Question)
                .Include(t => t.Status)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // GET: Transactions/Create
        public IActionResult Create(int id)
        {
            ViewData["ShowMessage"] = false;
            var transaction = new Transaction { QuestionId = id };
            return View(transaction);
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Transaction transaction, List<IFormFile> files)
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
            if (transaction.Answer == null || !ModelState.IsValid)
            {
                if (transaction.Answer == null)
                    ModelState.AddModelError(string.Empty, "Answer field is Required!");
                ViewData["ShowMessage"] = false;
                return View(transaction);
            }

            List<string> attatchments = new List<string>();
            if (files.Count > 0)
            {
                var pathToSave = Path.Combine("images", "answers");
                try
                {
                    attatchments = await FileHelper.UploadAll(files, pathToSave);
                }
                catch (Exception ex)
                {
                    return BadRequest("There is somthing wrong when adding the attachments!");
                }
            }

            var newTransaction = new Transaction
            {
                Answer = transaction.Answer,
                QuestionId = transaction.QuestionId,
                AcceptedBy = currentUser.Id,
                StatusId = 2,
                AnswerDate = DateTime.Now
            };

            var question = await _context.Questions.FindAsync(transaction.QuestionId);

            question.StatusId = 2;

            _context.Questions.Update(question);

            newTransaction.Answer = WebUtility.HtmlEncode(transaction.Answer);
            foreach (var attatchment in attatchments)
            {
                newTransaction.TransactionAttachments.Add(new TransactionAttachment { Path = attatchment });
            }

            ViewData["ShowMessage"] = true;
            _context.Add(newTransaction);
            await _context.SaveChangesAsync();
            return View(newTransaction);
        }

        // GET: Transactions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }
            ViewData["AcceptedBy"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", transaction.AcceptedBy);
            ViewData["QuestionId"] = new SelectList(_context.Questions, "Id", "Id", transaction.QuestionId);
            ViewData["StatusId"] = new SelectList(_context.Statuses, "Id", "Id", transaction.StatusId);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,QuestionId,AcceptedBy,Answer,AnswerDate,StatusId")] Transaction transaction)
        {
            if (id != transaction.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(transaction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransactionExists(transaction.Id))
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
            ViewData["AcceptedBy"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", transaction.AcceptedBy);
            ViewData["QuestionId"] = new SelectList(_context.Questions, "Id", "Id", transaction.QuestionId);
            ViewData["StatusId"] = new SelectList(_context.Statuses, "Id", "Id", transaction.StatusId);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .Include(t => t.Acceptor)
                .Include(t => t.Question)
                .Include(t => t.Status)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TransactionExists(int id)
        {
            return _context.Transactions.Any(e => e.Id == id);
        }
    }
}
