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

	[Authorize(Roles = $"{ConstData.Role_Admin_Name},{ConstData.Role_Mech_Name},{ConstData.Role_Engr_Name}")]
	public class ProjectsController : Controller
	{
		private readonly DrawingRegisterContext _context;
		private readonly IWebHostEnvironment _hostEnvironment;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly SignInManager<IdentityUser> _signInManager;

		public ProjectsController(
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

		


		public async Task<IActionResult> Index(string search, string states)
		{
			var user = await _userManager.GetUserAsync(User);
			var drawingRegisterUser = await _context.DrawingRegisterUsers.FirstOrDefaultAsync(dr => dr.UserId == user.Id);

			IQueryable<string> statesQuery = from s in _context.ProjectState 
											 where s.DrawingRegisterId == drawingRegisterUser!.DrawingRegisterId 
											 orderby s.Name select s.Name;

			var project = from p in _context.Project.Include(s => s.ProjectState)
						  where p.ProjectState!.DrawingRegisterId == drawingRegisterUser!.DrawingRegisterId 
						  select p;

			// If user is not administrator - eleminate projects with defined state
			if (drawingRegisterUser!.Role != ConstData.Role_Admin_Name)
			{
				project = project.Where(p => p.ProjectState!.Name != ConstData.State_Defined);
			}

			// Select projects that matches the search criterias
			if (search != null) 
			{ 
				project = project.Where(p => p.Name.Contains(search) || p.ProjectNubmer.Contains(search));
			}

			if (states != null)
			{
				project = project.Where(p => p.ProjectState!.Name == states);
			}

			// ProjectStateVM gathers Projects and search data
			var projectVM = new ProjectVM
			{
				ProjectStates = new SelectList(await statesQuery.Distinct().ToListAsync()),
				Projects = await project.OrderBy(p => p.ProjectNubmer).ToListAsync()
			};

			await _signInManager.RefreshSignInAsync(user);

			return View(projectVM);
		}

		


		public async Task<IActionResult> Details(int id)
		{
			var user = await _userManager.GetUserAsync(User);
			var drawingRegisterUser = await _context.DrawingRegisterUsers.FirstOrDefaultAsync(dr => dr.UserId == user.Id);
			var project = await _context.Project
				.Include(p => p.ProjectState)
				.Where(p => p.ProjectState!.DrawingRegisterId == drawingRegisterUser!.DrawingRegisterId)
				.FirstOrDefaultAsync(p => p.Id == id);

			// Check if project exists and ensure that user access project that is only in his DrawingRegister
			if ( project == null)
			{
				return NotFound();
			}

			// ProjectVM gathers all drawings, layouts and documentation that belongs to current Project
			var drawings = from d in _context.Drawing where d.ProjectId == id select d;
			var documentations = from d in _context.Documentation where d.ProjectId == id select d;
			var layouts = from l in _context.Layout where l.ProjectId == id select l;

			var ProjectVM = new ProjectVM
			{
				Project = project,
				Drawings = await drawings.OrderBy(d => d.DrawingNumber).ToListAsync(),
				Documentations = await documentations.OrderBy(d => d.FileName).ToListAsync(),
				Layouts = await layouts.OrderByDescending(l => l.CreateDate).ToListAsync()
			};

			return View(ProjectVM);
		}




		public async Task<IActionResult> Create()
		{
			// Return selectList of ProjectStates from current users DrawingRegister
			var user = await _userManager.GetUserAsync(User);
			var drawingRegisterUser = await _context.DrawingRegisterUsers.FirstOrDefaultAsync(d => d.UserId == user.Id);

			ViewData["ProjectStateId"] = new SelectList(
				_context.ProjectState.Where(s => s.DrawingRegisterId == drawingRegisterUser!.DrawingRegisterId), "Id", "Name");

			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = ConstData.Role_Admin_Name)]
		public async Task<IActionResult> Create(Project project)
		{
			var user = await _userManager.GetUserAsync(User);
			var drawingRegisterUser = await _context.DrawingRegisterUsers.FirstOrDefaultAsync(d => d.UserId == user.Id);

			// Prevent from same project number
			var existingProjects = await _context.Project
				.Include(p => p.ProjectState)
				.Where(p => p.ProjectState!.DrawingRegisterId == drawingRegisterUser!.DrawingRegisterId)
				.ToListAsync();

			if (project.ProjectNubmer == null || project.Name == null || project.Description == null)
			{
				ModelState.AddModelError("WhiteSpaces",
						"Fields should not be white spaces alone.");
			}
			else
			{
				foreach (var obj in existingProjects)
				{
					if (obj.ProjectNubmer.ToLower() == project.ProjectNubmer!.Trim().ToLower())
					{
						ModelState.AddModelError("ExistingProjectNumber",
							"This project number already exists. Please choose another number.");
					}
				}
			}

			if (ModelState.IsValid)
			{
				_context.Add(project);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}

			// If model state not valid return input data and selectList of ProjectStates from current users DrawingRegister
			ViewData["ProjectStateId"] = new SelectList(
				_context.ProjectState.Where(s => s.DrawingRegisterId == drawingRegisterUser!.DrawingRegisterId), 
				"Id", "Name", project.ProjectStateId);

			return View(project);
		}

		


		public async Task<IActionResult> Edit(int id)
		{
			var user = await _userManager.GetUserAsync(User);
			var drawingRegisterUser = await _context.DrawingRegisterUsers.FirstOrDefaultAsync(dr => dr.UserId == user.Id);
			var project = await _context.Project
				.Include(p => p.ProjectState)
				.Where(p => p.ProjectState!.DrawingRegisterId == drawingRegisterUser!.DrawingRegisterId)
				.FirstOrDefaultAsync(p => p.Id == id);

			// Check if project exists and ensure that user access project that is only in his DrawingRegister
			if ( project == null)
			{
				return NotFound();
			}

			// Return selectList of ProjectStates from current users DrawingRegister
			ViewData["ProjectStateId"] = new SelectList(
				_context.ProjectState.Where(s => s.DrawingRegisterId == drawingRegisterUser!.DrawingRegisterId), 
				"Id", "Name", project.ProjectStateId);

			return View(project);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = $"{ConstData.Role_Admin_Name},{ConstData.Role_Engr_Name}")]
		public async Task<IActionResult> Edit(int id, Project project)
		{
			var user = await _userManager.GetUserAsync(User);
			var drawingRegisterUser = await _context.DrawingRegisterUsers.FirstOrDefaultAsync(d => d.UserId == user.Id);

			// Prevent from same project number
			var existingProjects = await _context.Project
				.Include(p => p.ProjectState)
				.Where(p => p.ProjectState!.DrawingRegisterId == drawingRegisterUser!.DrawingRegisterId && p.Id != id)
				.ToListAsync();

			if (project.ProjectNubmer == null || project.Name == null || project.Description == null)
			{
				ModelState.AddModelError("WhiteSpaces",
						"Fields should not be white spaces alone.");
			}
			else
			{
				foreach (var obj in existingProjects)
				{
					if (obj.ProjectNubmer.ToLower() == project.ProjectNubmer!.Trim().ToLower())
					{
						ModelState.AddModelError("ExistingProjectNumber",
							"This project number already exists. Please choose another number.");
					}
				}
			}

			if (id != project.Id) 
			{ 
				return NotFound(); 
			}

			// Make sure that only administrator can change state
			var projectBeforEdit = await _context.Project.Include(p => p.ProjectState).FirstOrDefaultAsync(p => p.Id == id);

			if (drawingRegisterUser!.Role != ConstData.Role_Admin_Name)
			{
				if (projectBeforEdit!.ProjectStateId != project.ProjectStateId)
				{
					ModelState.AddModelError("OnlyAdmin",
						"Only the administrator has the ability to change the status of the project.");
				}
			}

			// Make sure that only administrator can edit if ProjectState is Defined, Completed or Canceled
			if(drawingRegisterUser!.Role != ConstData.Role_Admin_Name)
			{
				if(projectBeforEdit!.ProjectState!.Name == ConstData.State_Defined ||
					projectBeforEdit!.ProjectState!.Name == ConstData.State_Completed ||
					projectBeforEdit!.ProjectState!.Name == ConstData.State_Canceled)
				{
					ModelState.AddModelError("NoEdit",
						$"Only the administrator has the ability to edit this projcet " +
						$"if project state is set to {projectBeforEdit!.ProjectState!.Name}.");
				}
			}

			if (ModelState.IsValid)
			{
				// Detach EF Core Entity from tracking same primary key value
				_context.Entry(projectBeforEdit!).State = EntityState.Detached;

				try
				{
					_context.Update(project);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!ProjectExists(project.Id)) return NotFound();
					else throw;
				}

				return RedirectToAction(nameof(Details), new { project.Id });
			}

			// Return selectList of ProjectStates from current users DrawingRegister
			ViewData["ProjectStateId"] = new SelectList(
				_context.ProjectState.Where(s => s.DrawingRegisterId == drawingRegisterUser!.DrawingRegisterId),
				"Id", "Name", project.ProjectStateId);

			return View(project);
		}




		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null || _context.Project == null)
			{
				return NotFound();
			}

			var user = await _userManager.GetUserAsync(User);
			var drawingRegisterUser = await _context.DrawingRegisterUsers.FirstOrDefaultAsync(dr => dr.UserId == user.Id);
			var project = await _context.Project
				.Include(p => p.ProjectState)
				.Where(p => p.ProjectState!.DrawingRegisterId == drawingRegisterUser!.DrawingRegisterId)
				.FirstOrDefaultAsync(p => p.Id == id);

			// Check if project exists and ensure that user access project that is only in his DrawingRegister
			if (project == null)
			{
				return NotFound();
			}

			return View(project);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = ConstData.Role_Admin_Name)]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var user = await _userManager.GetUserAsync(User);
			var drawingRegisterUser = await _context.DrawingRegisterUsers.FirstOrDefaultAsync(dr => dr.UserId == user.Id);

			if (_context.Project == null)
			{
				return Problem("Entity set 'DrawingRegisterContext.Project' is null.");
			}

			var project = await _context.Project
				.Include(p => p.ProjectState)
				.Where(p => p.ProjectState!.DrawingRegisterId == drawingRegisterUser!.DrawingRegisterId)
				.FirstOrDefaultAsync(p => p.Id == id);

			if (project != null)
			{
				_context.Project.Remove(project);
			}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}




		private bool ProjectExists(int id)
		{
			return _context.Project.Any(e => e.Id == id);
		}
	}
}