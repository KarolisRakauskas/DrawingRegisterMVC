using Aspose.Pdf;
using Aspose.Pdf.Devices;
using DrawingRegisterWeb.Data;
using DrawingRegisterWeb.Models;
using DrawingRegisterWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace DrawingRegisterWeb.Controllers
{
	// DrawingRegister - Creates a unique identifier

	// DrawingRegisterUsers - Creates a relationship with DrawingRegisters and AspNetUsers

	// ProjectState - Defines states for projects, creates a relationship with projects and DrawingRegisters,
	//				  restricts access to DrawingRegisterUsers that share the same DrawingRegister as the current user

	// Project - Holds main data about user project. Seperates and group drawings, documentation.
	//			 Creates the relationship for drawings, documentation and layouts to ProjectStates

	[Authorize(Roles = $"{ConstData.Role_Admin_Name},{ConstData.Role_Mech_Name},{ConstData.Role_Engr_Name}")]
	public class LayoutsController : Controller
	{
		private readonly DrawingRegisterContext _context;
		private readonly IWebHostEnvironment _hostEnvironment;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly SignInManager<IdentityUser> _signInManager;

		public LayoutsController(
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




		public async Task<IActionResult> Index(string search, string projects)
		{
			var user = await _userManager.GetUserAsync(User);
			var drawingRegisterUser = await _context.DrawingRegisterUsers.FirstOrDefaultAsync(dr => dr.UserId == user.Id);

			var layout = from d in _context.Layout.Include(p => p.Project).Include(s => s.Project.ProjectState)
								where d.Project.ProjectState!.DrawingRegisterId == drawingRegisterUser!.DrawingRegisterId
								select d;

			IQueryable<string> ProjectsQuery;

			// If user is not administrator - eleminate layouts with ProjectState defined
			if (drawingRegisterUser!.Role != ConstData.Role_Admin_Name)
			{
				ProjectsQuery = from p in _context.Project.Include(s => s.ProjectState)
								where p.ProjectState!.DrawingRegisterId == drawingRegisterUser!.DrawingRegisterId &&
								p.ProjectState.Name != ConstData.State_Defined
								orderby p.ProjectNubmer
								select p.ProjectNubmer + " " + p.Name;
				layout = layout.Where(d => d.Project!.ProjectState!.Name != ConstData.State_Defined);
			}
			else
			{
				ProjectsQuery = from p in _context.Project.Include(s => s.ProjectState)
								where p.ProjectState!.DrawingRegisterId == drawingRegisterUser!.DrawingRegisterId
								orderby p.ProjectNubmer
								select p.ProjectNubmer + " " + p.Name;
			}

			// Select Layouts that matches the search criterias
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

			// LayoutVM gathers Layouts and search data
			var layoutVM = new LayoutVM
			{
				ProjectSelectList = new SelectList(await ProjectsQuery.Distinct().ToListAsync()),
				Layouts = await layout.ToListAsync()
			};

			await _signInManager.RefreshSignInAsync(user);

			return View(layoutVM);
		}




		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = $"{ConstData.Role_Admin_Name},{ConstData.Role_Engr_Name}")]
		public async Task<IActionResult> Create(int projectId, List<IFormFile> files)
		{
			var user = await _userManager.GetUserAsync(User);
			var drawingRegisterUser = await _context.DrawingRegisterUsers.FirstOrDefaultAsync(dr => dr.UserId == user.Id);

			var project = await _context.Project
				.Include(d => d.ProjectState)
				.Where(d => d.ProjectState!.DrawingRegisterId == drawingRegisterUser!.DrawingRegisterId)
				.FirstOrDefaultAsync(d => d.Id == projectId);

			// Check if project exists and ensure that user access to project that is only in his DrawingRegister
			if (project == null)
			{
				return NotFound();
			}

			// Make sure that only administrator can create layout files if ProjectState is Defined, Completed or Canceled
			if (drawingRegisterUser!.Role != ConstData.Role_Admin_Name)
			{
				if (project!.ProjectState!.Name == ConstData.State_Defined ||
					project!.ProjectState!.Name == ConstData.State_Completed ||
					project!.ProjectState!.Name == ConstData.State_Canceled)
				{
					TempData["NoUploadLayout"] = $"Only the administrator has the ability to upload layout files to this project " +
						$"if project state is set to {project!.ProjectState!.Name}.";

					return RedirectToAction("Details", "Projects", new { id = projectId });
				}
			}

			if (ModelState.IsValid)
			{
				foreach (var file in files)
				{
					// Get new file path, Guid name and extension
					string wwwRootPath = _hostEnvironment.WebRootPath;
					string fileName = Guid.NewGuid().ToString();
					var uploads = Path.Combine(wwwRootPath, @"Files\Layouts");
					var extension = Path.GetExtension(file.FileName);

					// Create Layout - keeps data about uploaded file
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

					// If file is pdf type - create files thumbnail
					if (layout.FileType.ToLower() == "pdf")
					{
						var pdfDocument = new Document(Path.Combine(uploads, fileName + extension));
						int pageIndex = 1;
						var page = pdfDocument.Pages[pageIndex];

						using FileStream imageStream = new(Path.Combine(uploads, fileName + ".jpg"), FileMode.Create);

						var resolution = new Resolution(300);
						var jpegDevice = new JpegDevice(200, 200, resolution, 200);

						jpegDevice.Process(page, imageStream);
						imageStream.Close();
					}

					_context.Add(layout);
					await _context.SaveChangesAsync();
				}

				return RedirectToAction("Details", "Projects", new { id = projectId });
			}

			return RedirectToAction("Details", "Projects", new { id = projectId });
		}




		[Authorize(Roles = $"{ConstData.Role_Admin_Name},{ConstData.Role_Engr_Name}")]
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null || _context.Layout == null)
			{
				return NotFound();
			}

			var user = await _userManager.GetUserAsync(User);
			var drawingRegisterUser = await _context.DrawingRegisterUsers.FirstOrDefaultAsync(dr => dr.UserId == user.Id);
			var layout = await _context.Layout
				.Include(d => d.Project)
				.Include(d => d.Project.ProjectState)
				.Where(d => d.Project.ProjectState!.DrawingRegisterId == drawingRegisterUser!.DrawingRegisterId)
				.FirstOrDefaultAsync(d => d.Id == id);

			// Check if layout exists and ensure that user access to layout that is only in his DrawingRegister
			if (layout == null)
			{
				return NotFound();
			}

			// Return aproved ProjectId
			ViewData["ProjectId"] = layout.ProjectId;
			return View(layout);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = $"{ConstData.Role_Admin_Name},{ConstData.Role_Engr_Name}")]
		public async Task<IActionResult> Edit(int? id, Layout layout)
		{
			var user = await _userManager.GetUserAsync(User);
			var drawingRegisterUser = await _context.DrawingRegisterUsers.FirstOrDefaultAsync(dr => dr.UserId == user.Id);

			// Check if Layout exsist and id is correct
			if (id == null || layout == null || id != layout.Id)
			{
				return NotFound();
			}

			var layoutBeforeEdit = await _context.Layout
				.Include(d => d.Project)
				.Include(d => d.Project.ProjectState)
				.Where(d => d.Project.ProjectState!.DrawingRegisterId == drawingRegisterUser!.DrawingRegisterId)
				.FirstOrDefaultAsync(d => d.Id == id);

			// Check if Layout belongs to current users DrawingRegister
			if (layoutBeforeEdit == null)
			{
				return NotFound();
			}

			// Make sure that only administrator can edit if ProjectState is Defined, Completed or Canceled
			if (drawingRegisterUser!.Role != ConstData.Role_Admin_Name)
			{
				if (layoutBeforeEdit!.Project.ProjectState!.Name == ConstData.State_Defined ||
					layoutBeforeEdit!.Project.ProjectState!.Name == ConstData.State_Completed ||
					layoutBeforeEdit!.Project.ProjectState!.Name == ConstData.State_Canceled)
				{
					ModelState.AddModelError("NoEdit",
						$"Only the administrator has the ability to edit this layout file " +
						$"if project state is set to {layoutBeforeEdit!.Project.ProjectState!.Name}.");
				}
			}

			// Prevent from white space layout FileName and revision
			if (layout.FileName == null || layout.Revision == null)
			{
				ModelState.AddModelError("WhiteSpaces",
						"Fields should not be white spaces alone.");
			}

			if (ModelState.IsValid)
			{
				// Deatach EF Core Entity from tracking same primary key value
				_context.Entry(layoutBeforeEdit!).State = EntityState.Detached;

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
					else throw;
				}
				return RedirectToAction("Details", "Projects", new { id = layout.ProjectId });
			}

			// Return aproved ProjectId
			ViewData["ProjectId"] = layout.ProjectId;
			return View(layout);

		}




		[Authorize(Roles = $"{ConstData.Role_Admin_Name},{ConstData.Role_Engr_Name}")]
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null || _context.Layout == null)
			{
				return NotFound();
			}

			var user = await _userManager.GetUserAsync(User);
			var drawingRegisterUser = await _context.DrawingRegisterUsers.FirstOrDefaultAsync(dr => dr.UserId == user.Id);
			var layout = await _context.Layout
				.Include(d => d.Project)
				.Include(d => d.Project.ProjectState)
				.Where(d => d.Project.ProjectState!.DrawingRegisterId == drawingRegisterUser!.DrawingRegisterId)
				.FirstOrDefaultAsync(d => d.Id == id);

			// Check if layout exists and ensure that user access layout file that is only in his DrawingRegister
			if (layout == null)
			{
				return NotFound();
			}

			return View(layout);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = $"{ConstData.Role_Admin_Name},{ConstData.Role_Engr_Name}")]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			if (_context.Layout == null)
			{
				return Problem("Entity set 'DrawingRegisterContext.Layout'  is null.");
			}

			var user = await _userManager.GetUserAsync(User);
			var drawingRegisterUser = await _context.DrawingRegisterUsers.FirstOrDefaultAsync(dr => dr.UserId == user.Id);
			var layout = await _context.Layout
				.Include(d => d.Project)
				.Include(d => d.Project.ProjectState)
				.Where(d => d.Project.ProjectState!.DrawingRegisterId == drawingRegisterUser!.DrawingRegisterId)
				.FirstOrDefaultAsync(d => d.Id == id);

			// Check if layout exists and ensure that user access layout file that is only in his DrawingRegister
			if (layout != null)
			{
				// Make sure that only administrator can edit if ProjectState is Defined, Completed or Canceled
				if (drawingRegisterUser!.Role != ConstData.Role_Admin_Name)
				{
					if (layout!.Project.ProjectState!.Name == ConstData.State_Defined ||
						layout!.Project.ProjectState!.Name == ConstData.State_Completed ||
						layout!.Project.ProjectState!.Name == ConstData.State_Canceled)
					{
						TempData["NoDelete"] = $"Only the administrator has the ability to delete this layout file " +
							$"if project state is set to {layout!.Project.ProjectState!.Name}.";

						return View(layout);
					}
				}

				// Delete old file and thumbnail if it exsist
				var oldFilePath = Path.Combine(_hostEnvironment.WebRootPath, layout.FileUrl.TrimStart('\\'));

				if (System.IO.File.Exists(oldFilePath) && !oldFilePath.Contains("SeededData")) // Prevent from deleting SeededData files
				{
					System.IO.File.Delete(oldFilePath);

					if (layout.FileType == "pdf")
					{
						int oldFileEndIndex = oldFilePath.LastIndexOf(".");
						string thumbanilUrl = oldFilePath[..oldFileEndIndex];

						thumbanilUrl += ".jpg";

						System.IO.File.Delete(thumbanilUrl);
					}
				}

				_context.Layout.Remove(layout);

				await _context.SaveChangesAsync();

				return RedirectToAction("Details", "Projects", new { id = layout.ProjectId });

			} else return NotFound();
		}




		private bool LayoutExists(int id)
		{
			return _context.Layout.Any(e => e.Id == id);
		}
	}
}
