using Aspose.Pdf;
using Aspose.Pdf.Devices;
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
	public class LayoutsController : Controller
	{
		private readonly DrawingRegisterContext _context;
		private readonly IWebHostEnvironment _hostEnvironment;

		public LayoutsController(DrawingRegisterContext context, IWebHostEnvironment hostEnvironment)
		{
			_context = context;
			_hostEnvironment = hostEnvironment;
		}

		// GET: Layouts
		public async Task<IActionResult> Index(string search, string projects)
		{
			IQueryable<string> ProjectsQuery = from p in _context.Project orderby p.ProjectNubmer select p.ProjectNubmer + " " + p.Name;

			var layout = from l in _context.Layout.Include(p => p.Project) select l;

			if (search != null)
			{
				layout = layout.Where(l =>
				l.FileName!.Contains(search) ||
				l.FileType!.Contains(search) ||
				l.Project!.Name.Contains(search));
			}

			if (projects != null)
			{
				layout = layout.Where(d => d.Project!.ProjectNubmer + " " + d.Project!.Name == projects);
			}

			var layoutVM = new LayoutVM
			{
				ProjectSelectList = new SelectList(await ProjectsQuery.Distinct().ToListAsync()),
				Layouts = await layout.ToListAsync()
			};

			return View(layoutVM);
		}

		// POST: Layouts/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(string projectId, List<IFormFile> files)
		{
			if (ModelState.IsValid)
			{
				foreach (var file in files)
				{
					var project = _context.Project.FirstOrDefault(p => p.Id == Int32.Parse(projectId));

					string wwwRootPath = _hostEnvironment.WebRootPath;

					string fileName = Guid.NewGuid().ToString();
					var uploads = Path.Combine(wwwRootPath, @"Files\Layouts");
					var extension = Path.GetExtension(file.FileName);

					var layout = new Layout()
					{
						ProjectId = project!.Id,
						CreateDate = DateTime.Now,
						FileName = Path.GetFileNameWithoutExtension(file.FileName).ToString(),
						FileType = Path.GetExtension(file.FileName).ToString().Remove(0, 1),
						FileUrl = @"\Files\Layouts\" + fileName + extension
					};

					using (var fileStream = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
					{
						file.CopyTo(fileStream);
					}

					if (layout.FileType.ToLower() == "pdf")
					{
						var pdfDocument = new Document(Path.Combine(uploads, fileName + extension));
						int pageIndex = 1;
						var page = pdfDocument.Pages[pageIndex];

						using (FileStream imageStream = new FileStream(Path.Combine(uploads, fileName + ".jpg"), FileMode.Create))
						{

							var resolution = new Resolution(300);
							var jpegDevice = new JpegDevice(200, 200, resolution, 200);
							jpegDevice.Process(page, imageStream);
							imageStream.Close();
						}
					}

					_context.Add(layout);
					await _context.SaveChangesAsync();
				}

				return RedirectToAction("Details", "Projects", new { id = projectId });
			}

			return RedirectToAction("Details", "Projects", new { id = projectId });
		}

		// GET: Layouts/Edit
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null || _context.Layout == null)
			{
				return NotFound();
			}

			var layout = await _context.Layout.FindAsync(id);

			if (layout == null)
			{
				return NotFound();
			}
			ViewData["ProjectId"] = new SelectList(_context.Project, "Id", "Name", layout.ProjectId);
			return View(layout);
		}

		// POST: Layouts/Edit
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, Layout layout)
		{
			if (id != layout.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(layout);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!LayoutExists(layout.Id))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}
				return RedirectToAction("Details", "Projects", new { id = layout.ProjectId });
			}
			ViewData["ProjectId"] = new SelectList(_context.Project, "Id", "Name", layout.ProjectId);
			return View(layout);

		}

		// GET: Layouts/Delete
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null || _context.Layout == null)
			{
				return NotFound();
			}

			var layout = await _context.Layout
				.Include(d => d.Project)
				.FirstOrDefaultAsync(d => d.Id == id);
			if (layout == null)
			{
				return NotFound();
			}

			return View(layout);
		}

		// POST: Layouts/Delete
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			if (_context.Layout == null)
			{
				return Problem("Entity set 'DrawingRegisterContext.Layout'  is null.");
			}

			var layout = await _context.Layout.FindAsync(id);
			int projectId = layout!.ProjectId;

			if (layout != null)
			{
				_context.Layout.Remove(layout);

				var oldFilePath = Path.Combine(_hostEnvironment.WebRootPath, layout.FileUrl.TrimStart('\\'));
				int oldFileEndIndex = oldFilePath.LastIndexOf(".");
				string thumbanilUrl = oldFilePath.Substring(0, oldFileEndIndex);
				thumbanilUrl += ".jpg";

				if (System.IO.File.Exists(oldFilePath) && !oldFilePath.Contains("SeededData"))
				{
					System.IO.File.Delete(oldFilePath);
					System.IO.File.Delete(thumbanilUrl);
				}
			}

			await _context.SaveChangesAsync();
			return RedirectToAction("Details", "Projects", new { id = projectId });
		}
		private bool LayoutExists(int id)
		{
			return _context.Layout.Any(e => e.Id == id);
		}
	}
}
