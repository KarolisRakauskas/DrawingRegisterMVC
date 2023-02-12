using Aspose.Pdf.Devices;
using DrawingRegisterWeb.Data;
using DrawingRegisterWeb.Models;
using DrawingRegisterWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace DrawingRegisterWeb.Controllers
{
	public class DocumentationsController : Controller
	{
		private readonly DrawingRegisterContext _context;
		private readonly IWebHostEnvironment _hostEnvironment;

		public DocumentationsController(DrawingRegisterContext context, IWebHostEnvironment hostEnvironment)
		{
			_context = context;
			_hostEnvironment = hostEnvironment;
		}

		// GET: Documentations
		public async Task<IActionResult> Index(string search, string projects)
		{
			IQueryable<string> ProjectsQuery = from p in _context.Project orderby p.ProjectNubmer select p.ProjectNubmer + " " + p.Name;

			var documentation = from d in _context.Documentation.Include(p => p.Project) select d;

			if (search != null)
			{
				documentation = documentation.Where(d =>
				d.FileName!.Contains(search) ||
				d.FileType!.Contains(search) ||
				d.Project.Name.Contains(search));
			}

			if (projects != null)
			{
				documentation = documentation.Where(d => d.Project!.ProjectNubmer + " " + d.Project!.Name == projects);
			}

			var documentationVM = new DocumentationVM
			{
				ProjectSelectList = new SelectList(await ProjectsQuery.Distinct().ToListAsync()),
				Documentations = await documentation.ToListAsync()
			};

			return View(documentationVM);
		}

		// POST: Documentations/Create
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
					var uploads = Path.Combine(wwwRootPath, @"Files\Documentation");
					var extension = Path.GetExtension(file.FileName);

					var documentation = new Documentation()
					{
						ProjectId = project!.Id,
						CreateDate = DateTime.Now,
						FileName = Path.GetFileNameWithoutExtension(file.FileName).ToString(),
						FileType = Path.GetExtension(file.FileName).ToString().Remove(0, 1),
						FileUrl = @"\Files\Documentation\" + fileName + extension
					};

					using (var fileStream = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
					{
						file.CopyTo(fileStream);
					}

					if (documentation.FileType.ToLower() == "pdf")
					{
						var pdfDocument = new Aspose.Pdf.Document(Path.Combine(uploads, fileName + extension));
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

					_context.Add(documentation);
					await _context.SaveChangesAsync();
				}

				return RedirectToAction("Details", "Projects", new { id = projectId });
			}

			return RedirectToAction("Details", "Projects", new { id = projectId });
		}

		// GET: Documentations/Edit
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null || _context.Documentation == null)
			{
				return NotFound();
			}

			var documentation = await _context.Documentation.FindAsync(id);

			if (documentation == null)
			{
				return NotFound();
			}
			ViewData["ProjectId"] = new SelectList(_context.Project, "Id", "Name", documentation.ProjectId);
			return View(documentation);
		}

		// POST: Documentations/Edit
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, Documentation documentation)
		{
			if (id != documentation.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(documentation);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!DocumentationExists(documentation.Id))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}
				return RedirectToAction("Details", "Projects", new { id = documentation.ProjectId });
			}
			ViewData["ProjectId"] = new SelectList(_context.Project, "Id", "Name", documentation.ProjectId);
			return View(documentation);

		}

		// GET: Documentations/Delete
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null || _context.Documentation == null)
			{
				return NotFound();
			}

			var documentation = await _context.Documentation
				.Include(d => d.Project)
				.FirstOrDefaultAsync(d => d.Id == id);
			if (documentation == null)
			{
				return NotFound();
			}

			return View(documentation);
		}

		// POST: Documentations/Delete
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			if (_context.Documentation == null)
			{
				return Problem("Entity set 'DrawingRegisterContext.Documentation'  is null.");
			}

			var documentation = await _context.Documentation.FindAsync(id);
			int projectId = documentation!.ProjectId;


			if (documentation != null)
			{
				_context.Documentation.Remove(documentation);

				var oldFilePath = Path.Combine(_hostEnvironment.WebRootPath, documentation.FileUrl.TrimStart('\\'));
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
		private bool DocumentationExists(int id)
		{
			return _context.Documentation.Any(e => e.Id == id);
		}
	}
}
