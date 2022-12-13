using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DrawingRegisterWeb.Data;
using DrawingRegisterWeb.Models;

namespace DrawingRegisterWeb.Controllers
{
    public class ProjectItemsController : Controller
    {
        private readonly DrawingRegisterContext _context;

        public ProjectItemsController(DrawingRegisterContext context)
        {
            _context = context;
        }

        // GET: ProjectItems
        public async Task<IActionResult> Index()
        {
            var drawingRegisterContext = _context.ProjectItem.Include(p => p.Project);
            return View(await drawingRegisterContext.ToListAsync());
        }

        // GET: ProjectItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ProjectItem == null)
            {
                return NotFound();
            }

            var projectItem = await _context.ProjectItem
                .Include(p => p.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (projectItem == null)
            {
                return NotFound();
            }

            return View(projectItem);
        }

        // GET: ProjectItems/Create
        public IActionResult Create()
        {
            ViewData["ProjectId"] = new SelectList(_context.Project, "Id", "ProjcetNubmer");
            return View();
        }

        // POST: ProjectItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Number,Name,Description,ProjectId")] ProjectItem projectItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(projectItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProjectId"] = new SelectList(_context.Project, "Id", "ProjcetNubmer", projectItem.ProjectId);
            return View(projectItem);
        }

        // GET: ProjectItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ProjectItem == null)
            {
                return NotFound();
            }

            var projectItem = await _context.ProjectItem.FindAsync(id);
            if (projectItem == null)
            {
                return NotFound();
            }
            ViewData["ProjectId"] = new SelectList(_context.Project, "Id", "ProjcetNubmer", projectItem.ProjectId);
            return View(projectItem);
        }

        // POST: ProjectItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Number,Name,Description,ProjectId")] ProjectItem projectItem)
        {
            if (id != projectItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(projectItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectItemExists(projectItem.Id))
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
            ViewData["ProjectId"] = new SelectList(_context.Project, "Id", "ProjcetNubmer", projectItem.ProjectId);
            return View(projectItem);
        }

        // GET: ProjectItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ProjectItem == null)
            {
                return NotFound();
            }

            var projectItem = await _context.ProjectItem
                .Include(p => p.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (projectItem == null)
            {
                return NotFound();
            }

            return View(projectItem);
        }

        // POST: ProjectItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ProjectItem == null)
            {
                return Problem("Entity set 'DrawingRegisterContext.ProjectItem'  is null.");
            }
            var projectItem = await _context.ProjectItem.FindAsync(id);
            if (projectItem != null)
            {
                _context.ProjectItem.Remove(projectItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectItemExists(int id)
        {
            return _context.ProjectItem.Any(e => e.Id == id);
        }
    }
}
