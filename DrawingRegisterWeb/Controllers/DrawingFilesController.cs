using DrawingRegisterWeb.Data;
using DrawingRegisterWeb.Models;
using DrawingRegisterWeb.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace DrawingRegisterWeb.Controllers
{
	public class DrawingFilesController : Controller
	{
		private readonly DrawingRegisterContext _context;
		private readonly IWebHostEnvironment _hostEnvironment;

		public DrawingFilesController(DrawingRegisterContext context, IWebHostEnvironment hostEnvironment)
		{
			_context = context;
			_hostEnvironment = hostEnvironment;
		}

		// POST: DrawingFiles/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(string drawingId, List<IFormFile> files)
		{
			if (ModelState.IsValid)
			{
				foreach (var file in files)
				{
					var drawing = _context.Drawing.FirstOrDefault(p => p.Id == Int32.Parse(drawingId));

					string wwwRootPath = _hostEnvironment.WebRootPath;

					string fileName = Guid.NewGuid().ToString();
					var uploads = Path.Combine(wwwRootPath, @"Files\Drawings");
					var extension = Path.GetExtension(file.FileName);

					var drawingFile = new DrawingFile()
					{
						DrawingId = drawing!.Id,
						CreateDate = DateTime.Now,
						FileName = Path.GetFileNameWithoutExtension(file.FileName).ToString(),
						FileType = Path.GetExtension(file.FileName).ToString().Remove(0, 1),
						FileUrl = @"\Files\Drawings\" + fileName + extension
					};

					using (var fileStream = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
					{
						file.CopyTo(fileStream);
					}

					_context.Add(drawingFile);
					await _context.SaveChangesAsync();
				}

				return RedirectToAction("Details", "Drawings", new { id = drawingId });
			}

			return RedirectToAction("Details", "Drawings", new { id = drawingId });
		}

		// GET: DrawingFiles/Edit
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null || _context.DrawingFile == null)
			{
				return NotFound();
			}

			var drawingFile = await _context.DrawingFile.FindAsync(id);

			if (drawingFile == null)
			{
				return NotFound();
			}
			ViewData["DrawingId"] = new SelectList(_context.Drawing, "Id", "DrawingNumber", drawingFile.DrawingId);
			return View(drawingFile);
		}

		// POST: DrawingFiles/Edit
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, DrawingFile drawingFile)
		{
			if (id != drawingFile.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(drawingFile);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!DrawingFileExists(drawingFile.Id))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}
				return RedirectToAction("Details", "Drawings", new { id = drawingFile.DrawingId });
			}
			ViewData["DrawingId"] = new SelectList(_context.Drawing, "Id", "DrawingNumber", drawingFile.DrawingId);
			return View(drawingFile);

		}

		// GET: DrawingFiles/Delete
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null || _context.DrawingFile == null)
			{
				return NotFound();
			}

			var layout = await _context.DrawingFile
				.Include(d => d.Drawing)
				.FirstOrDefaultAsync(d => d.Id == id);
			if (layout == null)
			{
				return NotFound();
			}

			return View(layout);
		}

		// POST: DrawingFiles/Delete
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			if (_context.DrawingFile == null)
			{
				return Problem("Entity set 'DrawingRegisterContext.DrawingFile'  is null.");
			}

			var drawingFile = await _context.DrawingFile.FindAsync(id);
			int drawingId = drawingFile!.DrawingId;

			if (drawingFile != null)
			{
				_context.DrawingFile.Remove(drawingFile);

				var oldFilePath = Path.Combine(_hostEnvironment.WebRootPath, drawingFile.FileUrl.TrimStart('\\'));

				if (System.IO.File.Exists(oldFilePath))
				{
					System.IO.File.Delete(oldFilePath);
				}

				_context.DrawingFile.Remove(drawingFile);

			}

			await _context.SaveChangesAsync();
			return RedirectToAction("Details", "Drawings", new { id = drawingId });
		}
		private bool DrawingFileExists(int id)
		{
			return _context.DrawingFile.Any(e => e.Id == id);
		}
	}
}
