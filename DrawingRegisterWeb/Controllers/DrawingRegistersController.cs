using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DrawingRegisterWeb.Data;
using DrawingRegisterWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using DrawingRegisterWeb.ViewModels;

namespace DrawingRegisterWeb
{
	[Authorize]
	public class DrawingRegistersController : Controller
	{
		private readonly DrawingRegisterContext _context;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly SignInManager<IdentityUser> _signInManager;

		public DrawingRegistersController(
			DrawingRegisterContext context, 
			UserManager<IdentityUser> userManager, 
			SignInManager<IdentityUser> signInManager)
		{
			_context = context;
			_userManager = userManager;
			_signInManager = signInManager;
		}




		public async Task<IActionResult> Index()
		{
			var registerVM = new RegisterVM();
			var user = _userManager.GetUserId(User);
			var userRegister = await _context.DrawingRegisterUsers.FirstOrDefaultAsync(u => u.UserId == user);

			if (userRegister == null) 
			{ 
				return View(registerVM); 
			}

			var drawingRegisterUsers = 
				from d in _context.DrawingRegisterUsers.Include(u => u.IdentityUser)
				where d.DrawingRegisterId == userRegister.DrawingRegisterId 
				select d;

			registerVM.DrawingRegister = await _context.DrawingRegisters.FirstOrDefaultAsync(d => d.Id == userRegister.DrawingRegisterId);
			registerVM.DrawingRegisterUsers = await drawingRegisterUsers.ToListAsync();

			return View(registerVM);
		}




		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(DrawingRegister drawingRegister)
		{
			var user = await _userManager.GetUserAsync(User);

			if (ModelState.IsValid && user != null)
			{
				_context.Add(drawingRegister);
				_context.SaveChanges();

				var drawingRegisterUser = new DrawingRegisterUsers
				{
					DrawingRegisterId = drawingRegister.Id,
					UserId = user.Id,
					Role = ConstData.Role_Admin_Name
				};

				//Seed required data for new drawing register - default states
				var states = SeedDataRuntime.CreateProjectStates(drawingRegister.Id);
				await _context.ProjectState.AddRangeAsync(states);
				await _context.SaveChangesAsync();

				//Seed example data for new drawing register: projects, drawings, layouts
				var projects = SeedDataRuntime.CreateProjects(states[0], states[1], drawingRegister.Id);
				await _context.Project.AddRangeAsync(projects);
				await _context.SaveChangesAsync();

				await _context.AddAsync(drawingRegisterUser);
				await _userManager.AddToRoleAsync(user!, ConstData.Role_Admin_Name);
				await _context.SaveChangesAsync();
				await _signInManager.RefreshSignInAsync(user!);

				return RedirectToAction(nameof(Index));
			}

			return View(drawingRegister);
		}




		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null || _context.DrawingRegisters == null)
			{
				return NotFound();
			}

			var drawingRegister = await _context.DrawingRegisters.FirstOrDefaultAsync(m => m.Id == id);
			if (drawingRegister == null)
			{
				return NotFound();
			}

			return View(drawingRegister);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var user = await _userManager.GetUserAsync(User);

			if (_context.DrawingRegisters == null)
			{
				return Problem("Entity set 'DrawingRegisterContext.DrawingRegisters'  is null.");
			}

			var drawingRegister = await _context.DrawingRegisters.FindAsync(id);

			if (drawingRegister != null)
			{
				_context.DrawingRegisters.Remove(drawingRegister);
			}

			//Remove all roles for all users in this current drawing register
			var drawingRegisterUsers = from d in _context.DrawingRegisterUsers where d.DrawingRegisterId == id select d;
			var registerUsersList = await drawingRegisterUsers.ToListAsync();
			var allUsers = from u in _userManager.Users select u;

			foreach (var item in registerUsersList)
			{
				var itemUser = await allUsers.FirstOrDefaultAsync(u => u.Id == item.UserId);
				await _userManager.RemoveFromRoleAsync(itemUser!, ConstData.Role_Admin_Name);
				await _userManager.RemoveFromRoleAsync(itemUser!, ConstData.Role_Engr_Name);
				await _userManager.RemoveFromRoleAsync(itemUser!, ConstData.Role_Mech_Name);
			}

			await _context.SaveChangesAsync();
			await _signInManager.RefreshSignInAsync(user!);
			return RedirectToAction(nameof(Index));
		}
	}
}
