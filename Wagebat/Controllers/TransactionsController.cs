﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
            Transaction transactionInput;
            if (transaction.Answer == null || !ModelState.IsValid)
            {
                if (transaction.Answer == null)
                    ModelState.AddModelError(string.Empty, "Answer field is Required!");
                ViewData["ShowMessage"] = false;
                return View(transaction);
            }

            List<FileWithType> attatchments = new List<FileWithType>();
            if (files.Count > 0)
            {
                var pathToSave = Path.Combine("images", "answers");
                try
                {
                    attatchments = await FileHelper.UploadAllWithType(files, pathToSave);
                }
                catch (Exception ex)
                {
                    return BadRequest("There is somthing wrong when adding the attachments!");
                }
            }
            var transactionInDb = _context.Transactions
                .Where(t => t.QuestionId == transaction.QuestionId)
                .FirstOrDefault();

            if(transactionInDb != null)
            {
                var status = await _context.Statuses.FindAsync(2);
                if (status == null)
                    return BadRequest("Add your default statuses first!");
                transactionInDb.Answer = transaction.Answer;
                transactionInDb.QuestionId = transaction.QuestionId;
                transactionInDb.AcceptedBy = currentUser.Id;
                transactionInDb.Status = status;
                transactionInDb.AnswerDate = DateTime.Now;
                transactionInput = transactionInDb;
            }
            else
            {
                var newTransaction = new Transaction
                {
                    Answer = transaction.Answer,
                    QuestionId = transaction.QuestionId,
                    AcceptedBy = currentUser.Id,
                    StatusId = 2,
                    AnswerDate = DateTime.Now
                };
                transactionInput = newTransaction;
            }

            var questionInDb = await _context.Questions.FindAsync(transaction.QuestionId);
            questionInDb.StatusId = 2;
            _context.Questions.Update(questionInDb);
            await _context.SaveChangesAsync();
            var stat = _context.Questions.FindAsync(transaction.QuestionId);

            transactionInput.Answer = WebUtility.HtmlEncode(transaction.Answer);
            foreach (var attatchment in attatchments)
            {
                transactionInput.TransactionAttachments.Add(new TransactionAttachment { Path = attatchment.Path, IsImage = attatchment.IsImage });
            }

            ViewData["ShowMessage"] = true;
            if (transactionInDb != null)
                _context.Transactions.Update(transactionInput);
            else
                _context.Add(transactionInput);
            await _context.SaveChangesAsync();
            return View(transactionInput);
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
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<JsonResult> DeleteConfirmed(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
                return Json(false);

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
            return Json(true);
        }


        public async Task<IActionResult> ApplyQuestion(int id)
        {
            var question = await _context.Questions.Include(q => q.Status).FirstOrDefaultAsync(q => q.Id == id);
            var status = await _context.Statuses.FindAsync(3);
            question.Status = status;
            
            var transaction = new Transaction
            {
                Question = question,
                Acceptor = await _userManager.FindByNameAsync(User.Identity.Name),
                StatusId = 3
            };
            _context.Questions.Update(question);
            await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
            return RedirectToAction("InstructorIndex", "Questions");
        }

        private bool TransactionExists(int id)
        {
            return _context.Transactions.Any(e => e.Id == id);
        }
    }
}
