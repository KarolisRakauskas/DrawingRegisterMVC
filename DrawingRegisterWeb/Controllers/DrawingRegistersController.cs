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

		//GET: DrawingRegisters
		public async Task<IActionResult> Index()
		{
			var user = _userManager.GetUserId(User);
			var userRegister = await _context.DrawingRegisterUsers.FirstOrDefaultAsync(u => u.UserId == user);

			if (userRegister == null)
			{
				return View();
			}

			var drawingRegisterUsers = 
				from d in _context.DrawingRegisterUsers.Include(u => u.IdentityUser)
				where d.DrawingRegisterId == userRegister.DrawingRegisterId 
				select d;

			var registerVM = new RegisterVM
			{
				DrawingRegister = await _context.DrawingRegisters.FirstOrDefaultAsync(d => d.Id == userRegister.DrawingRegisterId),
				DrawingRegisterUsers = await drawingRegisterUsers.ToListAsync()
			};

			return View(registerVM);
		}

		// GET: DrawingRegisters/Create
		public IActionResult Create()
		{
			return View();
		}

		// POST: DrawingRegisters/Create
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
					Role = "Administrator"
				};

				_context.Add(drawingRegisterUser);

				var states = SeedDataRuntime.CreateProjectStates(drawingRegister.Id);

				foreach (var state in states)
				{
					await _context.ProjectState.AddAsync(state);
				}

				await _userManager.AddToRoleAsync(user!, "Administrator");
				await _context.SaveChangesAsync();
				await _signInManager.RefreshSignInAsync(user!);

				return RedirectToAction(nameof(Index));
			}

			return View(drawingRegister);
		}

		// GET: DrawingRegisters/Delete/5
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

		// POST: DrawingRegisters/Delete/5
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

			//Take all Users and clear all roles
			var drawingRegisterUsers = from d in _context.DrawingRegisterUsers where d.DrawingRegisterId == id select d;
			var registerUsersList = await drawingRegisterUsers.ToListAsync();
			var allUsers = from u in _userManager.Users select u;

			foreach (var item in registerUsersList)
			{
				var itemUser = await allUsers.FirstOrDefaultAsync(u => u.Id == item.UserId);
				await _userManager.RemoveFromRoleAsync(itemUser!, "Administrator");
				await _userManager.RemoveFromRoleAsync(itemUser!, "Engineer");
				await _userManager.RemoveFromRoleAsync(itemUser!, "Mechanic");
			}

			await _context.SaveChangesAsync();
			await _signInManager.RefreshSignInAsync(user!);
			return RedirectToAction(nameof(Index));
		}
	}
}
