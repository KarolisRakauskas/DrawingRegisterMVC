using Aspose.Pdf.Devices;
using DrawingRegisterWeb.Data;
using DrawingRegisterWeb.Models;
using DrawingRegisterWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace DrawingRegisterWeb.Controllers
{
	// DrawingRegister - Creates a unique identifier

	// DrawingRegisterUsers - Creates a relationship with DrawingRegisters and AspNetUsers

	// ProjectState - Defines states for projects, creates a relationship with projects and DrawingRegisters,
	//				  restricts access to DrawingRegisterUsers that share the same DrawingRegister as the current user

	// Project - Holds main data about user project. Seperates and group drawings, documentation.
	//			 Creates the relationship for drawings, documentation and layouts to ProjectStates

	[Authorize(Roles = $"{ConstData.Role_Admin_Name},{ConstData.Role_Mech_Name},{ConstData.Role_Engr_Name}")]
	public class DocumentationsController : Controller
	{
		private readonly DrawingRegisterContext _context;
		private readonly IWebHostEnvironment _hostEnvironment;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly SignInManager<IdentityUser> _signInManager;

		public DocumentationsController(
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

			var documentation = from d in _context.Documentation.Include(p => p.Project).Include(s => s.Project.ProjectState)
						  where d.Project.ProjectState.DrawingRegisterId == drawingRegisterUser!.DrawingRegisterId
						  select d;

			IQueryable<string> ProjectsQuery;

			// If user is not administrator - eleminate documentation with ProjectState defined
			if (drawingRegisterUser!.Role != ConstData.Role_Admin_Name)
			{
				ProjectsQuery = from p in _context.Project.Include(s => s.ProjectState)
								where p.ProjectState.DrawingRegisterId == drawingRegisterUser!.DrawingRegisterId &&
								p.ProjectState.Name != ConstData.State_Defined
								orderby p.ProjectNubmer
								select p.ProjectNubmer + " " + p.Name;
				documentation = documentation.Where(d => d.Project.ProjectState.Name != ConstData.State_Defined);
			}
			else
			{
				ProjectsQuery = from p in _context.Project.Include(s => s.ProjectState)
								where p.ProjectState.DrawingRegisterId == drawingRegisterUser!.DrawingRegisterId
								orderby p.ProjectNubmer
								select p.ProjectNubmer + " " + p.Name;
			}

			// Select Documentation that matches the search criterias
			if (search != null)
			{
				documentation = documentation.Where(d =>
				d.FileName.Contains(search) ||
				d.FileType.Contains(search) ||
				d.Project.Name.Contains(search));
			}

			if (projects != null)
			{
				documentation = documentation.Where(d => d.Project.ProjectNubmer + " " + d.Project.Name == projects);
			}

			// DocumentationVM gathers Documentation and search data
			var documentationVM = new DocumentationVM
			{
				ProjectSelectList = new SelectList(await ProjectsQuery.Distinct().ToListAsync()),
				Documentations = await documentation.ToListAsync()
			};

			await _signInManager.RefreshSignInAsync(user);

			return View(documentationVM);
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
				.Where(d => d.ProjectState.DrawingRegisterId == drawingRegisterUser!.DrawingRegisterId)
				.FirstOrDefaultAsync(d => d.Id == projectId);

			// Check if project exists and ensure that user access to project that is only in his DrawingRegister
			if (project == null)
			{
				return NotFound();
			}

			// Make sure that only administrator can create drawing files if ProjectState is Defined, Completed or Canceled
			if (drawingRegisterUser!.Role != ConstData.Role_Admin_Name)
			{
				if (project!.ProjectState.Name == ConstData.State_Defined ||
					project!.ProjectState.Name == ConstData.State_Completed ||
					project!.ProjectState.Name == ConstData.State_Canceled)
				{
					TempData["NoUploadDocument"] = $"Only the administrator has the ability to upload documentation files to this project " +
						$"if project state is set to {project!.ProjectState.Name}.";

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
					var uploads = Path.Combine(wwwRootPath, @"Files\Documentation");
					var extension = Path.GetExtension(file.FileName);

					// Create Documentation - keeps data about uploaded file
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

					// If file is pdf type - create files thumbnail
					if (documentation.FileType.ToLower() == "pdf")
					{
						var pdfDocument = new Aspose.Pdf.Document(Path.Combine(uploads, fileName + extension));
						int pageIndex = 1;
						var page = pdfDocument.Pages[pageIndex];

						using FileStream imageStream = new(Path.Combine(uploads, fileName + ".jpg"), FileMode.Create);
						var resolution = new Resolution(300);
						var jpegDevice = new JpegDevice(200, 200, resolution, 200);

						jpegDevice.Process(page, imageStream);
						imageStream.Close();
					}

					_context.Add(documentation);
					await _context.SaveChangesAsync();
				}

				return RedirectToAction("Details", "Projects", new { id = projectId });
			}

			return RedirectToAction("Details", "Projects", new { id = projectId });
		}




		[Authorize(Roles = $"{ConstData.Role_Admin_Name},{ConstData.Role_Engr_Name}")]
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null || _context.Documentation == null)
			{
				return NotFound();
			}

			var user = await _userManager.GetUserAsync(User);
			var drawingRegisterUser = await _context.DrawingRegisterUsers.FirstOrDefaultAsync(dr => dr.UserId == user.Id);
			var documentation = await _context.Documentation
				.Include(d => d.Project)
				.Include(d => d.Project.ProjectState)
				.Where(d => d.Project.ProjectState.DrawingRegisterId == drawingRegisterUser!.DrawingRegisterId)
				.FirstOrDefaultAsync(d => d.Id == id);

			// Check if documentation file exists and ensure that user access to documentation that is only in his DrawingRegister
			if (documentation == null)
			{
				return NotFound();
			}

			// Return aproved ProjectId
			ViewData["ProjectId"] = documentation.ProjectId;
			return View(documentation);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = $"{ConstData.Role_Admin_Name},{ConstData.Role_Engr_Name}")]
		public async Task<IActionResult> Edit(int? id, Documentation documentation)
		{
			var user = await _userManager.GetUserAsync(User);
			var drawingRegisterUser = await _context.DrawingRegisterUsers.FirstOrDefaultAsync(dr => dr.UserId == user.Id);

			// Check if Documentation exsist and id is correct
			if (id == null || documentation == null || id != documentation.Id)
			{
				return NotFound();
			}

			var documentationBeforeEdit = await _context.Documentation
				.Include(d => d.Project)
				.Include(d => d.Project.ProjectState)
				.Where(d => d.Project.ProjectState.DrawingRegisterId == drawingRegisterUser!.DrawingRegisterId)
				.FirstOrDefaultAsync(d => d.Id == id);

			// Check if Documentation belongs to current users DrawingRegister
			if (documentationBeforeEdit == null)
			{
				return NotFound();
			}

			// Make sure that only administrator can edit if ProjectState is Defined, Completed or Canceled
			if (drawingRegisterUser!.Role != ConstData.Role_Admin_Name)
			{
				if (documentationBeforeEdit!.Project.ProjectState.Name == ConstData.State_Defined ||
					documentationBeforeEdit!.Project.ProjectState.Name == ConstData.State_Completed ||
					documentationBeforeEdit!.Project.ProjectState.Name == ConstData.State_Canceled)
				{
					ModelState.AddModelError("NoEdit",
						$"Only the administrator has the ability to edit this document " +
						$"if project state is set to {documentationBeforeEdit!.Project.ProjectState.Name}.");
				}
			}

			// Prevent from white space documentation FileName and revision
			if (documentation.FileName == null || documentation.Revision == null)
			{
				ModelState.AddModelError("WhiteSpaces",
						"Fields should not be white spaces alone.");
			}

			if (ModelState.IsValid)
			{
				// Deatach EF Core Entity from tracking same primary key value
				_context.Entry(documentationBeforeEdit!).State = EntityState.Detached;

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
					else throw;
				}
				return RedirectToAction("Details", "Projects", new { id = documentation.ProjectId });
			}

			// Return aproved ProjectId
			ViewData["ProjectId"] = documentation.ProjectId;
			return View(documentation);

		}




		[Authorize(Roles = $"{ConstData.Role_Admin_Name},{ConstData.Role_Engr_Name}")]
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null || _context.Documentation == null)
			{
				return NotFound();
			}

			var user = await _userManager.GetUserAsync(User);
			var drawingRegisterUser = await _context.DrawingRegisterUsers.FirstOrDefaultAsync(dr => dr.UserId == user.Id);
			var documentation = await _context.Documentation
				.Include(d => d.Project)
				.Include(d => d.Project.ProjectState)
				.Where(d => d.Project.ProjectState.DrawingRegisterId == drawingRegisterUser!.DrawingRegisterId)
				.FirstOrDefaultAsync(d => d.Id == id);

			// Check if drawing file exists and ensure that user access drawing file that is only in his DrawingRegister
			if (documentation == null)
			{
				return NotFound();
			}

			return View(documentation);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = $"{ConstData.Role_Admin_Name},{ConstData.Role_Engr_Name}")]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			if (_context.Documentation == null)
			{
				return Problem("Entity set 'DrawingRegisterContext.Documentation'  is null.");
			}

			var user = await _userManager.GetUserAsync(User);
			var drawingRegisterUser = await _context.DrawingRegisterUsers.FirstOrDefaultAsync(dr => dr.UserId == user.Id);
			var documentation = await _context.Documentation
				.Include(d => d.Project)
				.Include(d => d.Project.ProjectState)
				.Where(d => d.Project.ProjectState.DrawingRegisterId == drawingRegisterUser!.DrawingRegisterId)
				.FirstOrDefaultAsync(d => d.Id == id);

			// Check if drawing file exists and ensure that user access drawing file that is only in his DrawingRegister
			if (documentation != null)
			{
				// Make sure that only administrator can edit if ProjectState is Defined, Completed or Canceled
				if (drawingRegisterUser!.Role != ConstData.Role_Admin_Name)
				{
					if (documentation!.Project.ProjectState.Name == ConstData.State_Defined ||
						documentation!.Project.ProjectState.Name == ConstData.State_Completed ||
						documentation!.Project.ProjectState.Name == ConstData.State_Canceled)
					{
						TempData["NoDelete"] = $"Only the administrator has the ability to delete this documentation file " +
							$"if project state is set to {documentation!.Project.ProjectState.Name}.";

						return View(documentation);
					}
				}

				// Delete old file and thumbnail if it exsist
				var oldFilePath = Path.Combine(_hostEnvironment.WebRootPath, documentation.FileUrl.TrimStart('\\'));

				if (System.IO.File.Exists(oldFilePath) && !oldFilePath.Contains("SeededData")) // Prevent from deleting SeededData files
				{
					System.IO.File.Delete(oldFilePath);

					if (documentation.FileType == "pdf")
					{
						int oldFileEndIndex = oldFilePath.LastIndexOf(".");
						string thumbanilUrl = oldFilePath[..oldFileEndIndex];

						thumbanilUrl += ".jpg";

						System.IO.File.Delete(thumbanilUrl);
					}
				}

				_context.Documentation.Remove(documentation);

				await _context.SaveChangesAsync();

				return RedirectToAction("Details", "Projects", new { id = documentation.ProjectId });

			} else return NotFound();
		}




		private bool DocumentationExists(int id)
		{
			return _context.Documentation.Any(e => e.Id == id);
		}
	}
}
