using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Wagebat.Data;
using Wagebat.Models;
using Wagebat.ViewModels;

namespace Wagebat.Controllers
{
    public class PackagesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PackagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Packages
        public async Task<IActionResult> Index()
        {
            return View(await _context.Packages.Include(p => p.PackageItems).ThenInclude(pi => pi.Item).ToListAsync());
        }

        public async Task<IActionResult> AdminIndex()
        {
            return View(await _context.Packages.Include(p => p.PackageItems).ToListAsync());
        }

        // GET: Packages/Details/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var package = await _context.Packages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (package == null)
            {
                return NotFound();
            }

            return View(package);
        }

        // GET: Packages/Create
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            ViewData["Items"] = new SelectList( _context.Items.ToList(), "Id", "Name");
            ViewData["Courses"] = new SelectList( _context.Courses.ToList(), "Id", "Name");
            return View();
        }

        // POST: Packages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create(PackageInput input)
        {
            if (!ModelState.IsValid)
                return View(input);
            var isIntersect = input.WithItemsIds.Intersect(input.WithoutItemsIds).Count() > 0;
            if (isIntersect)
            {
                ModelState.AddModelError(string.Empty, "You shouldn't choose the same item twice!");
                ViewData["Items"] = new SelectList(_context.Items.ToList(), "Id", "Name");
                ViewData["Courses"] = new SelectList(_context.Courses.ToList(), "Id", "Name");
                return View(input);
            }
            var package = new Package
            {
                Name = input.Name,
                Description = input.Description,
                PriceAfter = input.PriceAfter,
                PriceBefore = input.PriceBefore,
                QuestionsCount = input.QuestionsCount
            };
            
            if (input.WithItemsIds != null && input.WithItemsIds.Count > 0)
            {
                foreach(var item in input.WithItemsIds)
                {
                    package.PackageItems.Add(new PackageItem
                    {
                        ItemId = item,
                        IsWith = true
                    });
                }
            }

            if (input.WithoutItemsIds != null && input.WithoutItemsIds.Count > 0)
            {
                foreach (var item in input.WithoutItemsIds)
                {
                    package.PackageItems.Add(new PackageItem
                    {
                        ItemId = item,
                    });
                }
            }

            if (input.CoursesIds != null && input.CoursesIds.Count > 0)
            {
                foreach (var item in input.CoursesIds)
                {
                    package.CoursePackages.Add(new CoursePackage
                    {
                        CourseId = item,
                    });
                }
            }
            _context.Add(package);
            await _context.SaveChangesAsync();
            
            return RedirectToAction(nameof(AdminIndex));
        }

        // GET: Packages/Edit/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var package = await _context.Packages
                .Include(p => p.CoursePackages)
                .Include(p => p.PackageItems)
                .ThenInclude(pi => pi.Item)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (package == null)
                return NotFound();

            var input = new PackageInput
            {
                Id = package.Id,
                Name = package.Name,
                Description = package.Description,
                PriceAfter = package.PriceAfter,
                PriceBefore = package.PriceBefore,
                QuestionsCount = package.QuestionsCount,
                CoursesIds = package.CoursePackages.Select(cp => cp.CourseId).ToList(),
                WithItemsIds = package.PackageItems.Where(pi => pi.IsWith).Select(pi => pi.ItemId).ToList(),
                WithoutItemsIds = package.PackageItems.Where(pi => pi.IsWith == false).Select(pi => pi.ItemId).ToList()
            };

            ViewData["Items"] = new SelectList(_context.Items.ToList(), "Id", "Name");
            ViewData["Courses"] = new SelectList(_context.Courses.ToList(), "Id", "Name");
            return View(input);
        }

        // POST: Packages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id, PackageInput input)
        {
            if (id != input.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(input);

            var isIntersect = input.WithItemsIds.Intersect(input.WithoutItemsIds).Count() > 0;
            if (isIntersect)
            {
                ModelState.AddModelError(string.Empty, "You shouldn't choose the same item twice!");
                ViewData["Items"] = new SelectList(_context.Items.ToList(), "Id", "Name");
                ViewData["Courses"] = new SelectList(_context.Courses.ToList(), "Id", "Name");
                return View(input);
            }
            var package = new Package
            {
                Id = input.Id,
                Name = input.Name,
                Description = input.Description,
                PriceAfter = input.PriceAfter,
                PriceBefore = input.PriceBefore,
                QuestionsCount = input.QuestionsCount
            };

            _context.PackageItems.RemoveRange(_context.PackageItems.Where(pi => pi.PackageId == input.Id));
            await _context.SaveChangesAsync();
                        
            if (input.WithItemsIds != null && input.WithItemsIds.Count > 0)
            {
                foreach (var item in input.WithItemsIds)
                {
                    _context.PackageItems.Add(new PackageItem { ItemId = item, IsWith = true, PackageId = package.Id });
                }
            }

            if (input.WithoutItemsIds != null && input.WithoutItemsIds.Count > 0)
            {
                foreach (var item in input.WithoutItemsIds)
                {
                    _context.PackageItems.Add(new PackageItem { ItemId = item, IsWith = false, PackageId = package.Id });
                }
            }

            if (input.CoursesIds != null && input.CoursesIds.Count > 0)
            {
                _context.CoursePackages.RemoveRange(_context.CoursePackages.Where(pi => pi.PackageId == input.Id));
                await _context.SaveChangesAsync();

                var range = await _context.Packages
                    .Include(p => p.CoursePackages)
                    .Where(p => p.Id == input.Id)
                    .Select(p => p.CoursePackages)
                    .SingleOrDefaultAsync();

                foreach (var item in input.CoursesIds)
                {
                    _context.CoursePackages.Add(new CoursePackage { CourseId = item, PackageId = package.Id });
                }
            }

            try
            {
                _context.Packages.Update(package);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PackageExists(input.Id))
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

        // GET: Packages/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var package = await _context.Packages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (package == null)
            {
                return NotFound();
            }

            return View(package);
        }

        // POST: Packages/Delete/5
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<JsonResult> DeleteConfirmed(int id)
        {
            var package = await _context.Packages.FindAsync(id);
            if (package == null)
                return Json(false);

            _context.Packages.Remove(package);
            await _context.SaveChangesAsync();
            return Json(true);
        }

        private bool PackageExists(int id)
        {
            return _context.Packages.Any(e => e.Id == id);
        }
    }
}
