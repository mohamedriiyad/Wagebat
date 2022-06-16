using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Wagebat.Data;
using Wagebat.Models;
using Wagebat.ViewModels.Users;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using Microsoft.EntityFrameworkCore;

namespace Wagebat.Controllers
{
    public class AdministrationController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;

        public AdministrationController(UserManager<ApplicationUser> userManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        // GET: AdministrationController
        [Authorize(Roles = "admin")]
        public ActionResult Index()
        {
            var users = _userManager.Users.Select(u => new UserListVM
            {
                UserName = u.UserName,
                Email = u.Email,
                Id = u.Id,
                PhoneNumber = u.PhoneNumber
            });

            return View(users);
        }

        [Authorize(Roles = "admin")]
        public IActionResult Create() => View();


        [Authorize(Roles = "instructor,admin")]
        public IActionResult CreateCourse() 
        {
            ViewData["Courses"] = new SelectList(_db.Courses, "Id", "Name");
            return View(); 
        }


        [HttpPost]
        [Authorize(Roles = "instructor,admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCourse(InstructorCourseInput input)
        { 
            if(!ModelState.IsValid)
            {
                ViewData["Courses"] = new SelectList(_db.Courses, "Id", "Name");
                return View();
            }

            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
            var userInDb = await _db.ApplicationUsers.Include(a => a.Courses).FirstOrDefaultAsync(u => u.Id == currentUser.Id);
            try
            {
                foreach (var courseId in input.CoursesIds)
                {
                    if (userInDb.Courses.Any(c => c.Id == courseId))
                        continue;
                    await _db.InstructorCourses.AddAsync(
                        new InstructorCourse { CourseId = courseId, InstuctorId = userInDb.Id }
                    );
                }
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "You musn't Add the same course twice!");
                ViewData["Courses"] = new SelectList(_db.Courses, "Id", "Name");
                return View();
            }
            if(User.IsInRole("admin"))
                return RedirectToAction("Index", "Courses");

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create(UserInputVM input)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = input.Username,
                    Email = input.Email,
                    PhoneNumber = input.PhoneNumber
                };
                var result = await _userManager.CreateAsync(user, input.Password);
                if (result.Succeeded)
                    return RedirectToAction("Index");

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(input);
        }
        // GET: AdministrationController/Details/5
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> Details(string id)
        {
            var userInDb = await FindByIdAsync(id);
            if(userInDb == null)
                return BadRequest("There is no such USER!");

            var user = new UserVM
            {
                UserName = userInDb.UserName,
                Email = userInDb.Email,
                PhoneNumber = userInDb.PhoneNumber
            };

            return View(user);
        }


        [Authorize(Roles = "admin")]
        // GET: AdministrationController/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            var userInDb = await FindByIdAsync(id);
            if(userInDb == null)
                return BadRequest("There is no such USER!");

            var user = new UserVM
            {
                Id = userInDb.Id,
                UserName = userInDb.UserName,
                Email = userInDb.Email
            };

            return View(user);
        }

        // POST: AdministrationController/Delete/5
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<JsonResult> DeleteConfirmed(string id)
        {
            var result = false;
            var user = await _userManager.FindByIdAsync(id);
            if(user != null)
            {
                result = true;
                await _userManager.DeleteAsync(user);
            }

            return Json(result);
        }

        [Authorize(Roles = "admin")]
        public async Task<ActionResult> Edit(string id)
        {
            var userInDb = await FindByIdAsync(id);
            if (userInDb == null)
                return BadRequest("There is no such USER!");
            var user = new UserEditVM
            {
                Id = userInDb.Id,
                Email = userInDb.Email,
                PhoneNumber = userInDb.PhoneNumber
            };

            return View(user);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(UserEditVM input)
        {
            if (!ModelState.IsValid)
            {
                return View(input);
            }

            var userInDb = await _db.ApplicationUsers.FindAsync(input.Id);
            userInDb.Email = input.Email;
            userInDb.PhoneNumber = input.PhoneNumber;
            await _db.SaveChangesAsync();

            var token = await _userManager.GeneratePasswordResetTokenAsync(userInDb);
            var changePasswordResult = await _userManager.ResetPasswordAsync(userInDb, token, input.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(input);
            }


            return RedirectToAction("Index");
        }

        [Authorize]
        public async Task<ActionResult> UserEdit()
        {
            var userInDb = await _userManager.FindByNameAsync(User.Identity.Name);
            if (userInDb == null)
                return BadRequest("There is no such USER!");
            var user = new UserSelfEditVM
            {
                Id = userInDb.Id,
                Email = userInDb.Email,
                PhoneNumber = userInDb.PhoneNumber
            };
           
            return View(user);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UserEdit(UserSelfEditVM input)
        {
            if (!ModelState.IsValid)
            {
                return View(input);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with Username '{_userManager.GetUserName(User)}'.");
            }
            var userInDb = await _db.ApplicationUsers.Include(a => a.Courses).FirstOrDefaultAsync(u => u.Id == user.Id);
            userInDb.Email = input.Email;
            userInDb.PhoneNumber = input.PhoneNumber;
            await _db.SaveChangesAsync();
            
            var changePasswordResult = await _userManager.ChangePasswordAsync(user, input.OldPassword, input.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(input);
            }

            if (User.IsInRole("instructor"))
            {
                var model = new InstructorCourseInput { CoursesIds = userInDb.Courses.Select(c => c.Id).ToList() };
                ViewData["Courses"] = new SelectList(_db.Courses, "Id", "Name");
                return View(nameof(CreateCourse), model);
            }
            return RedirectToAction("Index", "Home");
        }
        private async Task<ApplicationUser> FindByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }
    }
}
