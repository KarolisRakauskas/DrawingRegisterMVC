using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DrawingRegisterWeb.Data;
using DrawingRegisterWeb.Models;
using DrawingRegisterWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace DrawingRegisterWeb.Controllers
{
	[Authorize]
	public class ProjectStatesController : Controller
	{
		private readonly DrawingRegisterContext _context;

		public ProjectStatesController(DrawingRegisterContext context)
		{
			_context = context;
		}

		// GET: ProjectStates
		public async Task<IActionResult> Index(string search, string states)
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

			var projectStates = from p in _context.ProjectState select p;
			var drawingRegisters = from dr in _context.drawingRegisters select dr;
			var drawingRegister = drawingRegisters.FirstOrDefault(dr => dr.UserId == claim.Value);

			if (search != null)
			{
				projectStates = projectStates.Where(p =>
				p.Name.Contains(search) ||
				p.Description.Contains(search));
			}

			if (states != null)
			{
				if (states == "Standard")
				{
					projectStates = projectStates.Where(
						p => p.Name == "Defined" ||
						p.Name == "Running" ||
						p.Name == "Canceled" ||
						p.Name == "Completed"
						);
				} else if (states == "Custom")
				{
					projectStates = projectStates.Where(
						p => p.Name != "Defined" &&
						p.Name != "Running" &&
						p.Name != "Canceled" &&
						p.Name != "Completed"
						);
				}
			}

			var projectStateVM = new ProjectStateVM
			{
				ProjectStates = await projectStates.Where(p => p.DrawingRegisterId == drawingRegister.Id).OrderBy(p => p.Id).ToListAsync()
			};

			return View(projectStateVM);
		}

		// GET: ProjectStates/Create
		public IActionResult Create()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

			var drawingRegisters = from dr in _context.drawingRegisters select dr;
			var drawingRegister = drawingRegisters.FirstOrDefault(dr => dr.UserId == claim.Value);

			ProjectState projectState = new()
			{
				DrawingRegisterId = drawingRegister.Id
			};

			return View(projectState);
		}

		// POST: ProjectStates/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(ProjectState projectState)
		{
			if (ModelState.IsValid)
			{
				_context.Add(projectState);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			return View(projectState);
		}

		// GET: ProjectStates/Edit
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

		// POST: ProjectStates/Edit
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, ProjectState projectState)
		{
			if (id != projectState.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(projectState);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!ProjectStateExists(projectState.Id))
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
			return View(projectState);
		}

		// GET: ProjectStates/Delete
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null || _context.ProjectState == null)
			{
				return NotFound();
			}

			var projectState = await _context.ProjectState
				.FirstOrDefaultAsync(m => m.Id == id);
			if (projectState == null)
			{
				return NotFound();
			}

			return View(projectState);
		}

		// POST: ProjectStates/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			if (_context.ProjectState == null)
			{
				return Problem("Entity set is null.");
			}
			var projectState = await _context.ProjectState.FindAsync(id);
			if (projectState != null)
			{
				_context.ProjectState.Remove(projectState);
			}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool ProjectStateExists(int id)
		{
			return _context.ProjectState.Any(e => e.Id == id);
		}
	}
}
