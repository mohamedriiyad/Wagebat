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

        public IActionResult Create() => View();


        public IActionResult CreateCourse() 
        {
            ViewData["Courses"] = new SelectList(_db.Courses, "Id", "Name");
            return View(); 
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCourse(InstructorCourseInput input)
        { 
            if(!ModelState.IsValid)
            {
                ViewData["Courses"] = new SelectList(_db.Courses, "Id", "Name");
                return View();
            }

            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
            try
            {
                foreach (var courseId in input.CoursesIds)
                {
                    await _db.InstructorCourses.AddAsync(
                        new InstructorCourse { CourseId = courseId, InstuctorId = currentUser.Id }
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
            return RedirectToAction("Index", "Courses");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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

        //[HttpPost]
        //public async Task<IActionResult> Edit(UserEditVM input)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(input);
        //    }

        //    var userInDb = await _db.Users.FindAsync(input.Id);
        //    userInDb.Email = input.Email;
        //    userInDb.PhoneNumber = input.PhoneNumber;
        //    await _db.SaveChangesAsync();

        //    var token = await _userManager.GeneratePasswordResetTokenAsync(userInDb);
        //    var changePasswordResult = await _userManager.ResetPasswordAsync(userInDb, token, input.NewPassword); 
        //    if (!changePasswordResult.Succeeded)
        //    {
        //        foreach (var error in changePasswordResult.Errors)
        //        {
        //            ModelState.AddModelError(string.Empty, error.Description);
        //        }
        //        return View(input);
        //    }


        //    return RedirectToAction("Index");
        //}

        public async Task<ActionResult> UserEdit(string name)
        {
            var userInDb = await _userManager.FindByNameAsync(name);
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
            var userInDb = await _db.Users.FindAsync(user.Id);
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


            return RedirectToAction("Index", "Home");
        }
        private async Task<ApplicationUser> FindByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }
    }
}
