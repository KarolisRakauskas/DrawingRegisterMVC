using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DrawingRegisterWeb.Data;
using DrawingRegisterWeb.Models;
using DrawingRegisterWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace DrawingRegisterWeb.Controllers
{
	// DrawingRegisters - Creates a unique identifier
	// DrawingRegisterUsers - Creates a relationship with DrawingRegisters and AspNetUsers
	// ProjectState - Defines states for projects, creates a relationship with projects and DrawingRegisters,
	//				  restricts access to DrawingRegisterUsers that share the same DrawingRegister as the current user
	// Default states - States that are seeded into new DrawingRegister. No user can edit/delete default states

	[Authorize(Roles = ConstData.Role_Admin_Name)]
	public class ProjectStatesController : Controller
	{
		private readonly DrawingRegisterContext _context;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly SignInManager<IdentityUser> _signInManager;

		public ProjectStatesController(
			DrawingRegisterContext context,
			UserManager<IdentityUser> userManager,
			SignInManager<IdentityUser> signInManager)
		{
			_context = context;
			_userManager = userManager;
			_signInManager = signInManager;
		}




		public async Task<IActionResult> Index(string search, string states)
		{
			var user = await _userManager.GetUserAsync(User);
			var drawingRegisterUser = await _context.DrawingRegisterUsers.FirstOrDefaultAsync(dr => dr.UserId == user.Id);

			//Check if current user has DrawingRegister
			if (drawingRegisterUser == null) 
			{ 
				return RedirectToAction("Index", "DrawingRegisters");
			}

			var projectStates = from s in _context.ProjectState where s.DrawingRegisterId == drawingRegisterUser.DrawingRegisterId select s;

			//Select states that matches the search criterias
			if (search != null)
			{
				projectStates = projectStates.Where(p => p.Name.Contains(search) ||
					p.Description.Contains(search));
			}

			if (states != null)
			{
				if (states == "Standard")
				{
					projectStates = projectStates.Where(
						p => p.Name == ConstData.State_Defined ||
						p.Name == ConstData.State_Running ||
						p.Name == ConstData.State_Canceled ||
						p.Name == ConstData.State_Completed);
				}
				else if (states == "Custom")
				{
					projectStates = projectStates.Where(
						p => p.Name != ConstData.State_Defined &&
						p.Name != ConstData.State_Running &&
						p.Name != ConstData.State_Canceled &&
						p.Name != ConstData.State_Completed);
				}
			}

			//ProjectStateVM gathers ProjectStates and search data
			var projectStateVM = new ProjectStateVM
			{
				ProjectStates = await projectStates.OrderBy(p => p.Id).ToListAsync(),
				Search = search,
				States = states
			};

			await _signInManager.RefreshSignInAsync(user);

			return View(projectStateVM);
		}




		//Create custom state
		public async Task<IActionResult> Create()
		{
			var user = await _userManager.GetUserAsync(User);
			var drawingRegister = _context.DrawingRegisterUsers.FirstOrDefault(dr => dr.UserId == user.Id);

			if(drawingRegister != null)
			{
				ProjectState projectState = new()
				{
					DrawingRegisterId = drawingRegister!.DrawingRegisterId
				};

				return View(projectState);
			}

			return RedirectToAction("Index", "DrawingRegisters");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(ProjectState projectState)
		{
			var user = await _userManager.GetUserAsync(User);
			var drawingRegisterUser = _context.DrawingRegisterUsers.FirstOrDefault(dr => dr.UserId == user.Id);

			//Prevent from same ProjectState name
			var existingProjectStates = await _context.ProjectState
				.Where(s => s.DrawingRegisterId == drawingRegisterUser.DrawingRegisterId)
				.ToListAsync();

			foreach(var state in existingProjectStates)
			{
				if(state.Name.ToLower() == projectState.Name.Trim().ToLower())
				{
					ModelState.AddModelError("ExistingState",
						"This project state name already exists. Please choose another name.");
				}
			}

			if (ModelState.IsValid)
			{
				_context.Add(projectState);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}

			return View(projectState);
		}




		//Edit custom state
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null || _context.ProjectState == null)
			{
				return NotFound();
			}

			var projectState = await _context.ProjectState.FindAsync(id);

			if (projectState == null)
			{
				return NotFound();
			}

			return View(projectState);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, ProjectState projectState)
		{
			var user = await _userManager.GetUserAsync(User);

			//Check whether the current user's role has not been changed at this time
			var currentUserDrawingRegisterUser = await _context.DrawingRegisterUsers.FirstOrDefaultAsync(d => d.UserId == user.Id);

			if (currentUserDrawingRegisterUser!.Role != ConstData.Role_Admin_Name)
			{
				return RedirectToAction("Index", "DrawingRegisters");
			}

			if (id != projectState.Id)
			{
				return NotFound();
			}

			//Prevent from editing default state and from same ProjectState name






			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(projectState);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!ProjectStateExists(projectState.Id)) return NotFound();
					else throw;
				}
				return RedirectToAction(nameof(Index));
			}

			return View(projectState);
		}




		//ProjectStates/Delete
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null || _context.ProjectState == null) return NotFound();

			var projectState = await _context.ProjectState.FirstOrDefaultAsync(m => m.Id == id);

			if (projectState == null) return NotFound();

			return View(projectState);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			if (_context.ProjectState == null) return Problem("Entity set is null.");

			var projectState = await _context.ProjectState.FindAsync(id);

			if (projectState != null) _context.ProjectState.Remove(projectState);

			await _context.SaveChangesAsync();

			return RedirectToAction(nameof(Index));
		}

		private bool ProjectStateExists(int id)
		{
			return _context.ProjectState.Any(e => e.Id == id);
		}
	}
}
