using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Wagebat.Data;
using Wagebat.Helpers;
using Wagebat.Models;

namespace Wagebat.Controllers
{
    public class QuestionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public QuestionsController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Questions
        
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);

            var applicationDbContext = _context.Questions.Include(q => q.Status)
                .Include(q => q.Subscription)
                .Include(q => q.User)
                .Where(q => q.UserId == currentUser.Id);
            return View(await applicationDbContext.ToListAsync());
        }


        public async Task<IActionResult> TransactionIndex()
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);

            var transactions = _context.Transactions
                .Include(t => t.Status)
                .Include(t => t.Question)
                .Include(t => t.Acceptor)
                .Where(t => t.Acceptor.Id == currentUser.Id);

            foreach (var item in transactions)
            {
                item.Answer = WebUtility.HtmlDecode(item.Answer);
                item.Question.Body = WebUtility.HtmlDecode(item.Question.Body);
            }
            return View(await transactions.ToListAsync());
        }

        public async Task<IActionResult> StudentIndex()
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);

            var questions = await _context.Questions.Include(q => q.Status)
                .Include(q => q.Subscription)
                .Include(q => q.User)
                .Where(q => q.UserId == currentUser.Id).ToListAsync();
            foreach (var item in questions)
            {
                item.Body = WebUtility.HtmlDecode(item.Body);
            }
            return View(questions);
        }
        public async Task<IActionResult> InstructorIndex()
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
            var ids = _context.ApplicationUsers.SelectMany(u => u.Courses.Select(c => c.Id)).ToList();
            var questions = await _context.Questions
                .Include(q => q.Status)
                .Include(q => q.Subscription)
                .Include(q => q.User)
                .ThenInclude(u => u.Courses)
                .Where(q => ids.Any(id => id == q.CourseId) && q.StatusId == 1).ToListAsync();
            foreach (var item in questions)
            {
                item.Body = WebUtility.HtmlDecode(item.Body);
            }
            return View(questions);
        }


        // GET: Questions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var transaction = await _context.Transactions
                .Include(t => t.Acceptor)
                .Include(t => t.Comments)
                .Include(t => t.Question)
                .ThenInclude(q => q.QuestionAttachments)
                .Include(t => t.Question.Status)
                .Include(t => t.Reviews)
                .Include(t => t.Status)
                .Include(t => t.TransactionAttachments)
                .Where(t => t.QuestionId == id).FirstOrDefaultAsync();

            if (transaction == null)
            {
                var question = await _context.Questions
                .Include(q => q.Status)
                .Include(q => q.QuestionAttachments)
                .Include(q => q.Subscription)
                .Include(q => q.User)
                .FirstOrDefaultAsync(m => m.Id == id);
                if (question == null)
                    return NotFound("There is no such question!");

                transaction = new Transaction { Question = question };
            }
            transaction.Question.Body = WebUtility.HtmlDecode(transaction.Question.Body);
            transaction.Answer = WebUtility.HtmlDecode(transaction.Answer);
            return View(transaction);
        }

        // GET: Questions/Create
        public IActionResult Create()
        {
            ViewData["ShowMessage"] = false;
            ViewData["Courses"] = new SelectList(_context.Courses, "Id", "Name");
            return View();
        }

        // POST: Questions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Question question, List<IFormFile> files)
        {

            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
            var userSubscription = _context.Subscriptions
                .Where(s => s.UserId == currentUser.Id && s.User.Questions.Count < s.Package.QuestionsCount)
                .FirstOrDefault();
            if (userSubscription == null || question.Body == null)
            {
                if (userSubscription == null)
                    ModelState.AddModelError(string.Empty, "Sorry, You don't have any subscriptions Yet!");

                if (question.Body == null)
                    ModelState.AddModelError(string.Empty, "Question field is Required!");

                ViewData["Courses"] = new SelectList(_context.Courses, "Id", "Name");
                ViewData["ShowMessage"] = false;
                return View(question);
            }

            var pathToSave = Path.Combine("images", "questions");
            List<string> attatchments;
            try
            {
                attatchments = await FileHelper.UploadAll(files, pathToSave);
            }
            catch (Exception ex)
            {
                return BadRequest("You are tying to add a blog without an IMAGE!");
            }

            Question newQuestion = new Question
            {
                CourseId = question.CourseId,
                SubscriptionId = userSubscription.Id,
                Status = await _context.Statuses.FirstOrDefaultAsync(),
                UserId = currentUser.Id,
                Date = DateTime.Now
            };
            newQuestion.Body = WebUtility.HtmlEncode(question.Body);
            foreach(var attatchment in attatchments)
            {
                newQuestion.QuestionAttachments.Add(new QuestionAttachment { Path = attatchment });
            }
            _context.Add(newQuestion);
            await _context.SaveChangesAsync();

            ViewData["ShowMessage"] = true;
            ViewData["Courses"] = new SelectList(_context.Courses, "Id", "Name");
            return View();
        }

        // GET: Questions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var question = await _context.Questions.FindAsync(id);
            if (question == null)
            {
                return NotFound();
            }
            ViewData["StatusId"] = new SelectList(_context.Statuses, "Id", "Id", question.StatusId);
            ViewData["SubscriptionId"] = new SelectList(_context.Subscriptions, "Id", "UserId", question.SubscriptionId);
            ViewData["UserId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", question.UserId);
            return View(question);
        }

        // POST: Questions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CourseId,SubscriptionId,StatusId,UserId,Body,Date")] Question question)
        {
            if (id != question.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(question);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuestionExists(question.Id))
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
            ViewData["StatusId"] = new SelectList(_context.Statuses, "Id", "Id", question.StatusId);
            ViewData["SubscriptionId"] = new SelectList(_context.Subscriptions, "Id", "UserId", question.SubscriptionId);
            ViewData["UserId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", question.UserId);
            return View(question);
        }

        // GET: Questions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var question = await _context.Questions
                .Include(q => q.Status)
                .Include(q => q.Subscription)
                .Include(q => q.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (question == null)
            {
                return NotFound();
            }

            return View(question);
        }

        // POST: Questions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var question = await _context.Questions.FindAsync(id);
            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ApplyQuestion(int id)
        {
            var question = await _context.Questions.FindAsync(id);
            question.Status.Id = 3;
            _context.Questions.Update(question);
            await _context.SaveChangesAsync();
            return View();
        }



        private bool QuestionExists(int id)
        {
            return _context.Questions.Any(e => e.Id == id);
        }
    }
}