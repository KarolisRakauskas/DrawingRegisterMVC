using DrawingRegisterWeb.Data;
using DrawingRegisterWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using NuGet.Packaging.Signing;

namespace DrawingRegisterWeb.Controllers
{
	public class DrawingsController : Controller
	{
		private readonly DrawingRegisterContext _context;
		private readonly IWebHostEnvironment _hostEnvironment;

		public DrawingsController(DrawingRegisterContext context, IWebHostEnvironment hostEnvironment)
		{
			_context = context;
			_hostEnvironment = hostEnvironment;
		}

		// GET: DrawingsController
		public async Task<IActionResult> Index()
		{
			var drawingRegisterContext = _context.Drawing.Include(p => p.File);
			return View(await drawingRegisterContext.ToListAsync());
		}

		// GET: DrawingsController/Create
		public ActionResult Create()
		{
			return View();
		}

		// POST: DrawingsController/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create (Models.File File, Drawing Drawing, IFormFile? uploadFile)
		{
			string wwwRootPath = _hostEnvironment.WebRootPath;
			var uploads = Path.Combine(wwwRootPath, @"Files\Drawings");

			if (uploadFile != null)
			{
				File.FileName = Path.GetFileNameWithoutExtension(uploadFile.FileName).ToString();
				File.FileExtension = Path.GetExtension(uploadFile.FileName);

				using (var fileStream = new FileStream(Path.Combine(uploads, uploadFile.FileName), FileMode.Create))
				{
					uploadFile.CopyTo(fileStream);
				}

				File.FileUrl = @"\Files\Drawings\" + uploadFile.FileName;

				_context.Add(File);
				_context.SaveChanges();

				Drawing.DrawingNumber = File.FileName;
				Drawing.FileId = File.Id;

				_context.Add(Drawing);
				_context.SaveChanges();

				return RedirectToAction(nameof(Index));
			}

			return View();
		}

		// GET: Projects/Delete
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null || _context.Drawing == null)
			{
				return NotFound();
			}

			var drawing = await _context.Drawing
				.Include(p => p.File)
				.FirstOrDefaultAsync(m => m.Id == id);
			if (drawing == null)
			{
				return NotFound();
			}

			return View(drawing);
		}

		// POST: Projects/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			if (_context.Drawing == null || _context.File == null)
			{
				return Problem("Entity set 'DrawingRegisterContext.Drawing'  is null.");
			}
			var drawing = await _context.Drawing.FindAsync(id);
			var file = await _context.File.FindAsync(drawing.FileId);

			if (drawing != null || file != null)
			{
				_context.Drawing.Remove(drawing);

				var oldFilePath = Path.Combine(_hostEnvironment.WebRootPath, file.FileUrl.TrimStart('\\'));

				if (System.IO.File.Exists(oldFilePath))
				{
					System.IO.File.Delete(oldFilePath);
				}

				_context.File.Remove(file);

			}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool DrawingExists(int drawingid)
		{
			return _context.Drawing.Any(e => e.Id == drawingid);
		}
	}
}
