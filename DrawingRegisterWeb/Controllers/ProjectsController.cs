using DrawingRegisterWeb.Data;
using DrawingRegisterWeb.Models;
using DrawingRegisterWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DrawingRegisterWeb.Controllers
{
	public class ProjectsController : Controller
	{
		private readonly DrawingRegisterContext _context;

		public ProjectsController(DrawingRegisterContext context)
		{
			_context = context;
		}

		//GET: Projects
		public async Task<IActionResult> Index(string search, string states)
		{
			IQueryable<string> statesQuery = from s in _context.ProjectState orderby s.Name select s.Name;

			var project = from p in  _context.Project.Include(s => s.ProjectState) select p;

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
				Projects = await project.OrderBy(p => p.ProjectNubmer).ToListAsync(),
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

			var projectItems = from p in _context.ProjectItem where p.ProjectId == id select p;

			var Project_ProjectItemVM = new Project_ProjectItemVM
			{
				Project = project,
				ProjectItems = await projectItems.OrderBy(p => p.Number).ToListAsync()
			};

			return View(Project_ProjectItemVM);
		}

		// GET: Project/Create
		public IActionResult Create()
		{
			ViewData["ProjectStateId"] = new SelectList(_context.ProjectState, "Id", "Name");
			return View();
		}

		// POST: Project/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Id,ProjectNubmer,Name,Description,DeadlineDate,ProjectStateId")] Project project)
		{
			if (ModelState.IsValid)
			{
				_context.Add(project);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			ViewData["ProjectStateId"] = new SelectList(_context.ProjectState, "Id", "Name", project.ProjectStateId);
			return View(project);
		}
	}
}
