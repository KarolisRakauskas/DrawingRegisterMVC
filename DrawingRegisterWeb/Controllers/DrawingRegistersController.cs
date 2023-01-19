using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DrawingRegisterWeb.Data;
using DrawingRegisterWeb.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

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

		// GET: DrawingRegisters
		public async Task<IActionResult> Index()
		{
			var drawingRegisterContext = _context.DrawingRegisters.Include(d => d.IdentityUser);
			return View(await drawingRegisterContext.ToListAsync());
		}

		// GET: DrawingRegisters/Create
		public IActionResult Create()
		{
			var user = _userManager.GetUserId(User);
			var drawingRegister = new DrawingRegister
			{
				UserId = user
			};
			return View(drawingRegister);
		}

		// POST: DrawingRegisters/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Id,UserId")] DrawingRegister drawingRegister)
		{
			var user = await _userManager.GetUserAsync(User);

			if (ModelState.IsValid)
			{
				_context.Add(drawingRegister);
				_context.SaveChanges();

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

			ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", drawingRegister.UserId);
			return View(drawingRegister);
		}

		// GET: DrawingRegisters/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null || _context.DrawingRegisters == null)
			{
				return NotFound();
			}

			var drawingRegister = await _context.DrawingRegisters
				.Include(d => d.IdentityUser)
				.FirstOrDefaultAsync(m => m.Id == id);
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

			await _userManager.RemoveFromRoleAsync(user!, "Administrator");

			await _context.SaveChangesAsync();
			await _signInManager.RefreshSignInAsync(user!);
			return RedirectToAction(nameof(Index));
		}
	}
}
