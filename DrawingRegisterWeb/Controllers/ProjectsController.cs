using DrawingRegisterWeb.Data;
using DrawingRegisterWeb.Models;
using DrawingRegisterWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;

namespace DrawingRegisterWeb.Controllers
{
	public class ProjectsController : Controller
	{
		private readonly DrawingRegisterContext _context;
		private readonly IWebHostEnvironment _hostEnvironment;

		public ProjectsController(DrawingRegisterContext context, IWebHostEnvironment hostEnvironment)
		{
			_context = context;
			_hostEnvironment = hostEnvironment;
		}

		//GET: Projects
		public async Task<IActionResult> Index(string search, string states)
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity!;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
			var drawingRegisters = from dr in _context.DrawingRegisterUsers select dr;
			var drawingRegister = drawingRegisters.FirstOrDefault(dr => dr.UserId == claim!.Value);

			if (drawingRegister == null) return RedirectToAction("Index", "DrawingRegisters");

			IQueryable<string> statesQuery = from s in _context.ProjectState 
											 where s.DrawingRegisterId == drawingRegister.DrawingRegisterId 
											 orderby s.Name select s.Name;

			var project = from p in _context.Project.Include(s => s.ProjectState)
						  where p.ProjectState.DrawingRegisterId == drawingRegister.DrawingRegisterId 
						  select p;

			if (search != null) 
			{ 
				project = project.Where(p => 
				p.Name.Contains(search) || 
				p.ProjectNubmer.Contains(search));
			}

			if (states != null)
			{
				project = project.Where(p => p.ProjectState!.Name == states);
			}

			var projectVM = new ProjectVM
			{
				ProjectStates = new SelectList(await statesQuery.Distinct().ToListAsync()),
				Projects = await project.OrderBy(p => p.ProjectNubmer).ToListAsync()
			};

			return View(projectVM);
		}

		// GET: Project/Details
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null || _context.Project == null)
			{
				return NotFound();
			}

			var project = await _context.Project
				.Include(p => p.ProjectState)
				.FirstOrDefaultAsync(m => m.Id == id);

			if (project == null)
			{
				return NotFound();
			}

			var drawings = from d in _context.Drawing where d.ProjectId == id select d;
			var documentations = from d in _context.Documentation where d.ProjectId == id select d;
			var layouts = from l in _context.Layout where l.ProjectId == id select l;

			var ProjectVM = new ProjectVM
			{
				Project = project,
				Drawings = await drawings.OrderBy(d => d.DrawingNumber).ToListAsync(),
				Documentations = await documentations.OrderBy(d => d.FileName).ToListAsync(),
				Layouts = await layouts.OrderBy(l => l.FileName).ToListAsync()
			};

			return View(ProjectVM);
		}

		// GET: Project/Create
		public IActionResult Create()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity!;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
			var drawingRegisters = from dr in _context.DrawingRegisterUsers select dr;
			var drawingRegister = drawingRegisters.FirstOrDefault(dr => dr.UserId == claim!.Value);

			ViewData["ProjectStateId"] = new SelectList(
				_context.ProjectState.Where(s => s.DrawingRegisterId == drawingRegister.DrawingRegisterId), 
				"Id", 
				"Name");

			return View();
		}

		// POST: Project/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Id,ProjectNubmer,Name,Description,DeadlineDate,ProjectStateId")] Models.Project project)
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity!;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
			var drawingRegisters = from dr in _context.DrawingRegisterUsers select dr;
			var drawingRegister = drawingRegisters.FirstOrDefault(dr => dr.UserId == claim!.Value);

			if (ModelState.IsValid)
			{
				_context.Add(project);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}

			ViewData["ProjectStateId"] = new SelectList(
				_context.ProjectState.Where(s => s.DrawingRegisterId == drawingRegister.DrawingRegisterId), 
				"Id", 
				"Name", 
				project.ProjectStateId);

			return View(project);
		}

		// GET: Project/Edit
		public async Task<IActionResult> Edit(int? id)
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity!;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
			var drawingRegisters = from dr in _context.DrawingRegisterUsers select dr;
			var drawingRegister = drawingRegisters.FirstOrDefault(dr => dr.UserId == claim!.Value);

			if (id == null || _context.Project == null)
			{
				return NotFound();
			}

			var project = await _context.Project.FindAsync(id);
			if (project == null)
			{
				return NotFound();
			}

			ViewData["ProjectStateId"] = new SelectList(
				_context.ProjectState.Where(s => s.DrawingRegisterId == drawingRegister.DrawingRegisterId), 
				"Id", 
				"Name", 
				project.ProjectStateId);

			return View(project);
		}

		// POST: Project/Edit
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("Id,ProjectNubmer,Name,Description,CreateDate,DeadlineDate,ProjectStateId,ModelUrl")] Models.Project project)
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity!;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
			var drawingRegisters = from dr in _context.DrawingRegisterUsers select dr;
			var drawingRegister = drawingRegisters.FirstOrDefault(dr => dr.UserId == claim!.Value);

			if (id != project.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(project);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!ProjectExists(project.Id))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}
				return RedirectToAction(nameof(Details), new {id});
			}

			ViewData["ProjectStateId"] = new SelectList(
				_context.ProjectState.Where(s => s.DrawingRegisterId == drawingRegister.DrawingRegisterId), 
				"Id", 
				"Name",
				project.ProjectStateId);

			return View(project);
		}

		// GET: Project/Delete
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null || _context.Project == null)
			{
				return NotFound();
			}

			var project = await _context.Project
				.Include(p => p.ProjectState)
				.FirstOrDefaultAsync(m => m.Id == id);
			if (project == null)
			{
				return NotFound();
			}

			return View(project);
		}

		// POST: Project/Delete
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			if (_context.Project == null)
			{
				return Problem("Entity set 'DrawingRegisterContext.Project' is null.");
			}
			var project = await _context.Project.FindAsync(id);
			if (project != null)
			{
				_context.Project.Remove(project);
			}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		// POST: Project/Upload
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Upload(int id, IFormFile file)
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity!;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
			var drawingRegisters = from dr in _context.DrawingRegisterUsers select dr;
			var drawingRegister = drawingRegisters.FirstOrDefault(dr => dr.UserId == claim!.Value);

			var project = await _context.Project.FindAsync(id);

			if (id != project!.Id)
			{
				return NotFound();
			}

			string wwwRootPath = _hostEnvironment.WebRootPath;

			string fileName = Guid.NewGuid().ToString();
			var uploads = Path.Combine(wwwRootPath, @"Files\3DModels");
			var extension = Path.GetExtension(file.FileName)!.ToLower();

			if(project.ModelUrl != null)
			{
				var oldFilePath = Path.Combine(_hostEnvironment.WebRootPath, project.ModelUrl!.TrimStart('\\'));

				if (System.IO.File.Exists(oldFilePath) && !oldFilePath.Contains("SeededData"))
				{
					System.IO.File.Delete(oldFilePath);
				}
			}
	
			using (var fileStream = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
			{
				file.CopyTo(fileStream);
			}

			project.ModelUrl = @"\Files\3DModels\" + fileName + extension;

			try
			{
				_context.Update(project);
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!ProjectExists(project.Id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}

			ViewData["ProjectStateId"] = new SelectList(
				_context.ProjectState.Where(s => s.DrawingRegisterId == drawingRegister.DrawingRegisterId), 
				"Id", 
				"Name",
				project.ProjectStateId);
			return RedirectToAction("Edit", "Projects", new { id = project.Id });
		}

		// POST: Project/DeleteUpload
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> UploadDelete(int id)
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity!;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
			var drawingRegisters = from dr in _context.DrawingRegisterUsers select dr;
			var drawingRegister = drawingRegisters.FirstOrDefault(dr => dr.UserId == claim!.Value);

			var project = await _context.Project.FindAsync(id);

			if (project != null)
			{
				var oldFilePath = Path.Combine(_hostEnvironment.WebRootPath, project.ModelUrl!.TrimStart('\\'));

				if (System.IO.File.Exists(oldFilePath) && !oldFilePath.Contains("SeededData"))
				{
					System.IO.File.Delete(oldFilePath);
				}

				project!.ModelUrl = null;

				try
				{
					_context.Update(project);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!ProjectExists(project.Id))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}

				ViewData["ProjectStateId"] = new SelectList(
					_context.ProjectState.Where(s => s.DrawingRegisterId == drawingRegister.DrawingRegisterId), 
					"Id", 
					"Name",
					project.ProjectStateId);

				return RedirectToAction("Edit", "Projects", new { id = project.Id });
			}

				ViewData["ProjectStateId"] = new SelectList(
					_context.ProjectState.Where(s => s.DrawingRegisterId == drawingRegister.DrawingRegisterId), 
					"Id", 
					"Name",
					project.ProjectStateId);
			return RedirectToAction("Edit", "Projects", new { id = project.Id });
		}

		private bool ProjectExists(int id)
		{
			return _context.Project.Any(e => e.Id == id);
		}
	}
}
