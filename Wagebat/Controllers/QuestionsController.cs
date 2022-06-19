using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
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

            var transactions = await _context.Transactions
                .Include(t => t.Status)
                .Include(t => t.Question)
                .Include(t => t.Acceptor)
                .Where(t => t.Acceptor.Id == currentUser.Id && t.Question.StatusId == 2)
                .OrderByDescending(q => q.AnswerDate)
                .ToListAsync();

            foreach (var item in transactions)
            {
                item.Answer = WebUtility.HtmlDecode(item.Answer);
                item.Question.Body = WebUtility.HtmlDecode(item.Question.Body);
            }
            return View(transactions);
        }

        public async Task<IActionResult> StudentIndex()
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);

            var questions = await _context.Questions.Include(q => q.Status)
                .Include(q => q.Subscription)
                .Include(q => q.User)
                .Where(q => q.UserId == currentUser.Id)
                .OrderByDescending(q => q.Date)
                .ToListAsync();
            foreach (var item in questions)
            {
                item.Body = WebUtility.HtmlDecode(item.Body);
            }
            return View(questions);
        }
        public async Task<IActionResult> InstructorIndex()
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
            var ids = _context.ApplicationUsers
                .Where(u =>u.Id == currentUser.Id)
                .SelectMany(u => u.Courses.Select(c => c.Id))
                .ToList();

            var questions = await _context.Questions
                .Include(q => q.Status)
                .Where(q => ids.Any(id => id == q.CourseId) && q.StatusId == 1)
                .OrderByDescending(q => q.Date)
                .ToListAsync();

            foreach (var item in questions)
            {
                item.Body = WebUtility.HtmlDecode(item.Body);
            }
            return View(questions);
        }

        public async Task<IActionResult> AppliedIndex()
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);

            var ids = _context.ApplicationUsers
                .SelectMany(u => u.Courses.Select(c => c.Id))
                .ToList();
            List<Question> questions = new List<Question>();
            if (User.IsInRole("instructor"))
            {
                questions =  await _context.Transactions
                    .Include(t => t.Question)
                    .Include(t => t.Question.Status)
                    .Where(t => ids.Any(id => id == t.Question.CourseId) && t.Question.StatusId == 3 && t.AcceptedBy == currentUser.Id)
                    .Select(t => t.Question)
                    .OrderByDescending(q => q.Date)
                    .ToListAsync();
            }
            else if (User.IsInRole("user"))
            {
                questions = await _context.Questions
                .Include(q => q.Status)
                .Where(q => ids.Any(id => id == q.CourseId) && q.StatusId == 3 && q.UserId == currentUser.Id)
                .OrderByDescending(q => q.Date)
                .ToListAsync();
            }

            foreach (var item in questions)
            {
                item.Body = WebUtility.HtmlDecode(item.Body);
            }
            return View(questions);
        }


        // GET: Questions/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var transaction = await _context.Transactions
                .Include(t => t.Acceptor)
                .Include(t => t.Comments)
                .ThenInclude(t => t.User)
                .Include(t => t.Question)
                .ThenInclude(q => q.QuestionAttachments)
                .Include(t => t.Question.Status)
                .Include(t => t.Question.User)
                .Include(t => t.Review)
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
        [Authorize]
        public async Task<IActionResult> Create()
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
            var userSubscriptions = await _context.Subscriptions
                .Include(s => s.Confirmer)
                .Include(s => s.User.Questions)
                .Include(s => s.Package)
                .Where(s => s.UserId == currentUser.Id && s.Confirmer != null)
                .ToListAsync();
            var pakagesQCount = 0;
            var userQCount = 0;
            if(userSubscriptions.Count > 0)
            {
                userQCount = userSubscriptions.FirstOrDefault().User.Questions.Count;
                foreach (var subscription in userSubscriptions)
                {
                    pakagesQCount += subscription.Package.QuestionsCount;
                }
            }
            
            var courses = await _context.Subscriptions
                .Include(s => s.Package)
                .Where(s => s.UserId == currentUser.Id)
                .SelectMany(s => s.Package.CoursePackages)
                .Select(cp => cp.Course)
                .Distinct()
                .ToListAsync();
            var universities = await _context.Universities.ToListAsync();
            var levels = await _context.Levels.ToListAsync();

            ViewData["ShowMessage"] = false;
            ViewData["Message"] = "You have " + (pakagesQCount - userQCount) + " Questions Left!";
            ViewData["Courses"] = new SelectList(courses, "Id", "Name");
            ViewData["Universities"] = new SelectList(universities, "Id", "Name");
            ViewData["Levels"] = new SelectList(levels, "Id", "Name");
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
            var userSubscriptions = await _context.Subscriptions
                .Include(s => s.Confirmer)
                .Include(s => s.User.Questions)
                .Include(s => s.Package)
                .Where(s => s.UserId == currentUser.Id && s.Confirmer != null)
                .ToListAsync();
            
            var courses = await _context.Subscriptions
                .Include(s => s.Package)
                .Where(s => s.UserId == currentUser.Id)
                .SelectMany(s => s.Package.CoursePackages)
                .Select(cp => cp.Course).ToListAsync();
            var universities = await _context.Universities.ToListAsync();
            var levels = await _context.Levels.ToListAsync();

            if (userSubscriptions.Count < 1 || question.Body == null)
            {
                if (userSubscriptions.Count < 1)
                    ModelState.AddModelError(string.Empty, "Sorry, You don't have any available subscriptions! or Your subscription doesn't confirmed yet!");

                if (question.Body == null)
                    ModelState.AddModelError(string.Empty, "Question field is Required!");

                ViewData["ShowMessage"] = false;
                ViewData["Universities"] = new SelectList(universities, "Id", "Name");
                ViewData["Levels"] = new SelectList(levels, "Id", "Name");
                return View(question);
            }

            var pakagesQCount = 0;
            var userQCount = userSubscriptions.FirstOrDefault().User.Questions.Count;
            foreach (var subscription in userSubscriptions)
            {
                pakagesQCount += subscription.Package.QuestionsCount;
            }
            if(pakagesQCount - userQCount <= 0)
            {
                ModelState.AddModelError(string.Empty, "Sorry, You don't have any available subscriptions! or Your subscription doesn't confirmed yet!");

                ViewData["ShowMessage"] = false;
                ViewData["Universities"] = new SelectList(universities, "Id", "Name");
                ViewData["Levels"] = new SelectList(levels, "Id", "Name");
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
                SubscriptionId = userSubscriptions.FirstOrDefault().Id,
                Status = await _context.Statuses.FirstOrDefaultAsync(),
                UserId = currentUser.Id,
                Date = DateTime.Now
            };
            newQuestion.Body = WebUtility.HtmlEncode(question.Body);
            foreach (var attatchment in attatchments)
            {
                newQuestion.QuestionAttachments.Add(new QuestionAttachment { Path = attatchment });
            }
            _context.Add(newQuestion);
            await _context.SaveChangesAsync();

            ViewData["ShowMessage"] = true;
            ViewData["Message"] = "You have " + (pakagesQCount - userQCount - 1) + " Questions Left!";
            ViewData["Universities"] = new SelectList(universities, "Id", "Name");
            ViewData["Levels"] = new SelectList(levels, "Id", "Name");
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

        private bool QuestionExists(int id)
        {
            return _context.Questions.Any(e => e.Id == id);
        }
    }
}