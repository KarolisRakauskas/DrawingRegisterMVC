using DrawingRegisterWeb.Data;
using DrawingRegisterWeb.Models;
using DrawingRegisterWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DrawingRegisterWeb.Controllers
{
	// DrawingRegister - Creates a unique identifier

	// DrawingRegisterUsers - Creates a relationship with DrawingRegisters and AspNetUsers

	// ProjectState - Defines states for projects, creates a relationship with projects and DrawingRegisters,
	//				  restricts access to DrawingRegisterUsers that share the same DrawingRegister as the current user

	// Project - Holds main data about user project. Seperates and group drawings, documentation.
	//			 Creates the relationship for drawings, documentation and layouts to ProjectStates

	// Drawings - One of the building blocks of the project.
	//			  Holds main data about drawing files that belong to area/equipment/construction and so on.
	//			  Creates the relationship between DrawingFiles and project

	[Authorize(Roles = $"{ConstData.Role_Admin_Name},{ConstData.Role_Mech_Name},{ConstData.Role_Engr_Name}")]
	public class DrawingsController : Controller
	{
		private readonly DrawingRegisterContext _context;
		private readonly IWebHostEnvironment _hostEnvironment;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly SignInManager<IdentityUser> _signInManager;

		public DrawingsController(
			DrawingRegisterContext context,
			IWebHostEnvironment hostEnvironment,
			UserManager<IdentityUser> userManager,
			SignInManager<IdentityUser> signInManager)
		{
			_context = context;
			_hostEnvironment = hostEnvironment;
			_userManager = userManager;
			_signInManager = signInManager;
		}




		public async Task<IActionResult> Index(string search, string projects)
		{
			var user = await _userManager.GetUserAsync(User);
			var drawingRegisterUser = await _context.DrawingRegisterUsers.FirstOrDefaultAsync(dr => dr.UserId == user.Id);

			var drawing = from d in _context.Drawing.Include(p => p.Project).Include(s => s.Project.ProjectState) 
						  where d.Project.ProjectState!.DrawingRegisterId == drawingRegisterUser!.DrawingRegisterId
						  select d;

			IQueryable<string> ProjectsQuery;

			// If user is not administrator - eleminate drawings with ProjectState defined
			if (drawingRegisterUser!.Role != ConstData.Role_Admin_Name)
			{
				ProjectsQuery = from p in _context.Project.Include(s => s.ProjectState) 
												   where p.ProjectState!.DrawingRegisterId == drawingRegisterUser!.DrawingRegisterId && 
												   p.ProjectState.Name != ConstData.State_Defined 
												   orderby p.ProjectNubmer select p.ProjectNubmer + " " + p.Name;
				drawing = drawing.Where(d => d.Project.ProjectState!.Name != ConstData.State_Defined);
			}
			else
			{
				ProjectsQuery = from p in _context.Project.Include(s => s.ProjectState)
												   where p.ProjectState!.DrawingRegisterId == drawingRegisterUser!.DrawingRegisterId
												   orderby p.ProjectNubmer select p.ProjectNubmer + " " + p.Name;
			}

			// Select Drawings that matches the search criterias
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

			// DrawingVM gathers Drawings and search data
			var drawingVM = new DrawingVM
			{
				ProjectSelectList = new SelectList(await ProjectsQuery.Distinct().ToListAsync()),
				Drawings = await drawing.ToListAsync()
			};

			return View(drawingVM);
		}




		public async Task<IActionResult> Details(int id)
		{
			var user = await _userManager.GetUserAsync(User);
			var drawingRegisterUser = await _context.DrawingRegisterUsers.FirstOrDefaultAsync(dr => dr.UserId == user.Id);
			var drawing = await _context.Drawing
				.Include(d => d.Project)
				.Include(d => d.Project.ProjectState)
				.Where(d => d.Project.ProjectState!.DrawingRegisterId == drawingRegisterUser!.DrawingRegisterId)
				.FirstOrDefaultAsync(p => p.Id == id);
			

			// Check if drawing exists and ensure that user access drawing that is only in his DrawingRegister
			if (drawing == null)
			{
				return NotFound();
			}

			// DrawingVM gathers all DrawingFiles that belongs to current Drawing
			var drawingfiles = from d in _context.DrawingFile where d.DrawingId == id select d;

			var drawingVM = new DrawingVM
			{
				Drawing = drawing,
				DrawingFiles = drawingfiles.OrderBy(d => d.FileName).ToList()
			};

			return View(drawingVM);
		}





		//TODO
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
		[Authorize(Roles = $"{ConstData.Role_Admin_Name},{ConstData.Role_Engr_Name}")]
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
