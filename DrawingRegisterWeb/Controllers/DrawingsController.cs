using DrawingRegisterWeb.Data;
using DrawingRegisterWeb.Models;
using DrawingRegisterWeb.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Configuration;
using System.Security.Claims;

namespace DrawingRegisterWeb.Controllers
{
	public class DrawingsController : Controller
	{
		private readonly DrawingRegisterContext _context;

		public DrawingsController(DrawingRegisterContext context)
		{
			_context = context;
		}

		// GET: Drawings
		public async Task<IActionResult> Index(string search, string projects)
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity!;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
			var drawingRegisters = from dr in _context.DrawingRegisterUsers select dr;
			var drawingRegister = drawingRegisters.FirstOrDefault(dr => dr.UserId == claim!.Value);
			IQueryable<string> ProjectsQuery = from p in _context.Project.Include(s => s.ProjectState) 
											   where p.ProjectState!.DrawingRegisterId == drawingRegister!.DrawingRegisterId 
											   orderby p.ProjectNubmer select p.ProjectNubmer + " " + p.Name;

			var drawing = from d in _context.Drawing.Include(p => p.Project).Include(s => s.Project.ProjectState) 
						  where d.Project.ProjectState!.DrawingRegisterId == drawingRegister!.DrawingRegisterId
						  select d;

			if (search != null)
			{
				drawing = drawing.Where(d =>
				d.DrawingNumber.Contains(search) ||
				d.Name.Contains(search) ||
				d.Project.Name.Contains(search) ||
				d.Description!.Contains(search));
			}

			if (projects != null)
			{
				drawing = drawing.Where(d => d.Project!.ProjectNubmer + " " + d.Project!.Name == projects);
			}

			var drawingVM = new DrawingVM
			{
				ProjectSelectList = new SelectList(await ProjectsQuery.Distinct().ToListAsync()),
				Drawings = await drawing.ToListAsync()
			};

			return View(drawingVM);
		}

		// GET: Drawings/Details
		public ActionResult Details(int id)
		{
			var drawing = from d in _context.Drawing.Include(p => p.Project) select d;
			var drawingfiles = from d in _context.DrawingFile where d.DrawingId == id select d;

			var drawingVM = new DrawingVM
			{
				Drawing = drawing.FirstOrDefault(d => d.Id == id),
				DrawingFiles = drawingfiles.OrderBy(d => d.FileName).ToList()
			};

			return View(drawingVM);
		}

		// GET: Drawings/Create
		public IActionResult Create(string idData)
		{

			var project = _context.Project.FirstOrDefault(p => p.Id == Int32.Parse(idData));

			ViewData["ProjectId"] = new SelectList(_context.Project, "Id", "Name", project!.Id);
			ViewData["ProjectRouteId"] = idData;

			return View();
		}

		// POST: Drawings/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(Drawing drawing)
		{
			drawing.CreateDate = DateTime.Now;

			if (ModelState.IsValid)
			{
				_context.Add(drawing);
				await _context.SaveChangesAsync();
				return RedirectToAction("Details","Projects", new {id = drawing.ProjectId});
			}
			ViewData["ProjectId"] = new SelectList(_context.Project, "Id", "Name", drawing.ProjectId);
			return View(drawing);
		}

		// GET: Drawings/Edit
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null || _context.Drawing == null)
			{
				return NotFound();
			}

			var drawing = await _context.Drawing.FindAsync(id);

			if(drawing == null)
			{
				return NotFound();
			}

			ViewData["ProjectId"] = new SelectList(_context.Project, "Id", "Name", drawing.ProjectId);
			return View(drawing);
		}

		// POST: Drawings/Edit
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, Drawing drawing)
		{
			if( id != drawing.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(drawing);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!DrawingExists(drawing.Id))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}
				return RedirectToAction(nameof(Details), new { id });
			}

			ViewData["ProjectId"] = new SelectList(_context.Project, "Id", "Name", drawing.ProjectId);
			return View(drawing);

		}

		// GET: Drawings/Delete
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null || _context.Drawing == null)
			{
				return NotFound();
			}

			var drawing = await _context.Drawing
				.Include(d => d.Project)
				.FirstOrDefaultAsync(m => m.Id == id);
			if (drawing == null)
			{
				return NotFound();
			}

			return View(drawing);
		}

		// POST: Drawings/Delete
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			if (_context.Drawing == null)
			{
				return Problem("Entity set 'DrawingRegisterContext.Drawing'  is null.");
			}
			var drawing = await _context.Drawing.FindAsync(id);
			if (drawing != null)
			{
				_context.Drawing.Remove(drawing);
			}

			await _context.SaveChangesAsync();
			return RedirectToAction("Details", "Projects", new { id = drawing!.ProjectId }); ;
		}

		private bool DrawingExists(int id)
		{
			return _context.Drawing.Any(e => e.Id == id);
		}
	}
}
