using Aspose.Pdf;
using Aspose.Pdf.Devices;
using DrawingRegisterWeb.Data;
using DrawingRegisterWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DrawingRegisterWeb.Controllers
{
	// DrawingRegister - Creates a unique identifier

	// DrawingRegisterUsers - Creates a relationship with DrawingRegisters and AspNetUsers

	// ProjectState - Defines states for projects, creates a relationship with projects and DrawingRegisters,
	//				  restricts access to DrawingRegisterUsers that share the same DrawingRegister as the current user

	// Project - Holds main data about user project. Seperates and group drawings, documentation.
	//			 Creates the relationship for drawings, documentation and layouts to ProjectStates

	// Drawings - One of the building blocks of the project.
	//			  Holds main data about drawing files that belong to area/equipment/construction and so on.
	//			  Creates the relationship between DrawingFiles and project

	[Authorize(Roles = $"{ConstData.Role_Admin_Name},{ConstData.Role_Engr_Name}")]
	public class DrawingFilesController : Controller
	{
		private readonly DrawingRegisterContext _context;
		private readonly IWebHostEnvironment _hostEnvironment;
		private readonly UserManager<IdentityUser> _userManager;

		public DrawingFilesController(
			DrawingRegisterContext context,
			IWebHostEnvironment hostEnvironment,
			UserManager<IdentityUser> userManager)
		{
			_context = context;
			_hostEnvironment = hostEnvironment;
			_userManager = userManager;
		}




		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(int drawingId, List<IFormFile> files, bool automaticRevision = false)
		{
			var user = await _userManager.GetUserAsync(User);
			var drawingRegisterUser = await _context.DrawingRegisterUsers.FirstOrDefaultAsync(dr => dr.UserId == user.Id);

			var drawing = await _context.Drawing
				.Include(d => d.Project)
				.Include(d => d.Project.ProjectState)
				.Where(d => d.Project.ProjectState!.DrawingRegisterId == drawingRegisterUser!.DrawingRegisterId)
				.FirstOrDefaultAsync(d => d.Id == drawingId);

			// Check if drawing exists and ensure that user access to drawing that is only in his DrawingRegister
			if (drawing == null)
			{
				return NotFound();
			}

			// Make sure that only administrator can create drawing files if ProjectState is Defined, Completed or Canceled
			if (drawingRegisterUser!.Role != ConstData.Role_Admin_Name)
			{
				if (drawing!.Project.ProjectState!.Name == ConstData.State_Defined ||
					drawing!.Project.ProjectState!.Name == ConstData.State_Completed ||
					drawing!.Project.ProjectState!.Name == ConstData.State_Canceled)
				{
					TempData["NoUpload"] = $"Only the administrator has the ability to upload files to this drawing " +
						$"if project state is set to {drawing!.Project.ProjectState!.Name}.";

					return RedirectToAction("Details", "Drawings", new { id = drawingId });
				}
			}

			if (ModelState.IsValid)
			{
				foreach (var file in files)
				{
					// Get new file path, Guid name and extension
					string wwwRootPath = _hostEnvironment.WebRootPath;
					string fileName = Guid.NewGuid().ToString();
					var uploads = Path.Combine(wwwRootPath, @"Files\Drawings");
					var extension = Path.GetExtension(file.FileName)!.ToLower();

					// Create DrawingFile - Drawing file keeps data about uploaded file
					var drawingFile = new DrawingFile()
					{
						DrawingId = drawing!.Id,
						CreateDate = DateTime.Now,
						FileName = Path.GetFileNameWithoutExtension(file.FileName)!,
						FileType = Path.GetExtension(file.FileName)!.Remove(0, 1),
						FileUrl = @"\Files\Drawings\" + fileName + extension
					};

					// if automatic Revision checked - add revision to drawing file from file name
					if (automaticRevision)
					{
						if (file.FileName != null && file.FileName.Contains('_'))
						{
							int fileNameEndIndex = drawingFile.FileName.LastIndexOf('_');
							drawingFile.Revision = Path.GetFileNameWithoutExtension(file.FileName)!.Substring(fileNameEndIndex + 1);
						
						}
					}

					using (var fileStream = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
					{
						file.CopyTo(fileStream);
					}

					// If file is pdf type - create files thumbnail
					if(drawingFile.FileType.ToLower() == "pdf")
					{
						var pdfDocument = new Document(Path.Combine(uploads, fileName + extension));
						int pageIndex = 1;
						var page = pdfDocument.Pages[pageIndex];

						using (FileStream imageStream = new FileStream(Path.Combine(uploads,fileName + ".jpg"), FileMode.Create))
						{

							var resolution = new Resolution(300);
							var jpegDevice = new JpegDevice(200, 200, resolution, 200);
							jpegDevice.Process(page, imageStream);
							imageStream.Close();
						}
					}

					_context.Add(drawingFile);
					await _context.SaveChangesAsync();
				}

				return RedirectToAction("Details", "Drawings", new { id = drawingId });
			}

			return RedirectToAction("Details", "Drawings", new { id = drawingId });
		}




		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null || _context.DrawingFile == null)
			{
				return NotFound();
			}

			var user = await _userManager.GetUserAsync(User);
			var drawingRegisterUser = await _context.DrawingRegisterUsers.FirstOrDefaultAsync(dr => dr.UserId == user.Id);
			var drawingFile = await _context.DrawingFile
				.Include(d => d.Drawing)
				.Include(d => d.Drawing.Project)
				.Include(d => d.Drawing.Project.ProjectState)
				.Where(d => d.Drawing.Project.ProjectState!.DrawingRegisterId == drawingRegisterUser!.DrawingRegisterId)
				.FirstOrDefaultAsync(d => d.Id == id);

			// Check if drawing file exists and ensure that user access to drawing that is only in his DrawingRegister
			if (drawingFile == null)
			{
				return NotFound();
			}

			// Return aproved DrawingId
			ViewData["DrawingId"] = drawingFile.DrawingId;
			return View(drawingFile);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int? id, DrawingFile drawingFile)
		{
			var user = await _userManager.GetUserAsync(User);
			var drawingRegisterUser = await _context.DrawingRegisterUsers.FirstOrDefaultAsync(dr => dr.UserId == user.Id);

			// Check if drawingFile exsist and id is correct
			if (id == null || drawingFile == null || id != drawingFile.Id)
			{
				return NotFound();
			}

			var drawingFileBeforeEdit = await _context.DrawingFile
				.Include(d => d.Drawing)
				.Include(d => d.Drawing.Project)
				.Include(d => d.Drawing.Project.ProjectState)
				.Where(d => d.Drawing.Project.ProjectState!.DrawingRegisterId == drawingRegisterUser!.DrawingRegisterId)
				.FirstOrDefaultAsync(d => d.Id == id);

			// Check if DrawingFile belongs to current users DrawingRegister
			if(drawingFileBeforeEdit == null) 
			{ 
				return NotFound(); 
			}

			// Make sure that only administrator can edit if ProjectState is Defined, Completed or Canceled
			if (drawingRegisterUser!.Role != ConstData.Role_Admin_Name)
			{
				if (drawingFileBeforeEdit!.Drawing.Project.ProjectState!.Name == ConstData.State_Defined ||
					drawingFileBeforeEdit!.Drawing.Project.ProjectState!.Name == ConstData.State_Completed ||
					drawingFileBeforeEdit!.Drawing.Project.ProjectState!.Name == ConstData.State_Canceled)
				{
					ModelState.AddModelError("NoEdit",
						$"Only the administrator has the ability to edit this drawing file " +
						$"if project state is set to {drawingFileBeforeEdit!.Drawing.Project.ProjectState!.Name}.");
				}
			}

			// Prevent from white space drawing number and revision
			if (drawingFile.FileName == null || drawingFile.Revision == null)
			{
				ModelState.AddModelError("WhiteSpaces",
						"Fields should not be white spaces alone.");
			}

			if (ModelState.IsValid)
			{
				// Deatach EF Core Entity from tracking same primary key value
				_context.Entry(drawingFileBeforeEdit!).State = EntityState.Detached;

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
					else throw;
				}
				return RedirectToAction("Details", "Drawings", new { id = drawingFile.DrawingId });
			}

			// Return aproved DrawingId
			ViewData["DrawingId"] = drawingFile.DrawingId;
			return View(drawingFile);
		}




		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null || _context.DrawingFile == null)
			{
				return NotFound();
			}

			var user = await _userManager.GetUserAsync(User);
			var drawingRegisterUser = await _context.DrawingRegisterUsers.FirstOrDefaultAsync(dr => dr.UserId == user.Id);
			var drawingFile = await _context.DrawingFile
				.Include(d => d.Drawing)
				.Include(d => d.Drawing.Project)
				.Include(d => d.Drawing.Project.ProjectState)
				.Where(d => d.Drawing.Project.ProjectState!.DrawingRegisterId == drawingRegisterUser!.DrawingRegisterId)
				.FirstOrDefaultAsync(d => d.Id == id);

			// Check if drawing file exists and ensure that user access drawing file that is only in his DrawingRegister
			if (drawingFile == null)
			{
				return NotFound();
			}

			return View(drawingFile);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			if (_context.DrawingFile == null)
			{
				return Problem("Entity set 'DrawingRegisterContext.DrawingFile'  is null.");
			}

			var user = await _userManager.GetUserAsync(User);
			var drawingRegisterUser = await _context.DrawingRegisterUsers.FirstOrDefaultAsync(dr => dr.UserId == user.Id);
			var drawingFile = await _context.DrawingFile
				.Include(d => d.Drawing)
				.Include(d => d.Drawing.Project)
				.Include(d => d.Drawing.Project.ProjectState)
				.Where(d => d.Drawing.Project.ProjectState!.DrawingRegisterId == drawingRegisterUser!.DrawingRegisterId)
				.FirstOrDefaultAsync(d => d.Id == id);

			// Check if drawing file exists and ensure that user access drawing file that is only in his DrawingRegister
			if (drawingFile != null)
			{
				// Make sure that only administrator can edit if ProjectState is Defined, Completed or Canceled
				if (drawingRegisterUser!.Role != ConstData.Role_Admin_Name)
				{
					if (drawingFile!.Drawing.Project.ProjectState!.Name == ConstData.State_Defined ||
						drawingFile!.Drawing.Project.ProjectState!.Name == ConstData.State_Completed ||
						drawingFile!.Drawing.Project.ProjectState!.Name == ConstData.State_Canceled)
					{
						TempData["NoDelete"] = $"Only the administrator has the ability to edit this drawing file " +
							$"if project state is set to {drawingFile!.Drawing.Project.ProjectState!.Name}.";

						return View(drawingFile);
					}
				}

				// Delete old file and thumbnail if it exsist
				var oldFilePath = Path.Combine(_hostEnvironment.WebRootPath, drawingFile.FileUrl.TrimStart('\\'));

				if (System.IO.File.Exists(oldFilePath) && !oldFilePath.Contains("SeededData")) // Prevent from deleting SeededData files
				{
					System.IO.File.Delete(oldFilePath);

					if(drawingFile.FileType == "pdf")
					{
						int oldFileEndIndex = oldFilePath.LastIndexOf(".");
						string thumbanilUrl = oldFilePath.Substring(0, oldFileEndIndex);

						thumbanilUrl += ".jpg";

						System.IO.File.Delete(thumbanilUrl);
					}
				}

				_context.DrawingFile.Remove(drawingFile);

				await _context.SaveChangesAsync();

				return RedirectToAction("Details", "Drawings", new { id = drawingFile!.DrawingId });

			} else return NotFound();
		}




		private bool DrawingFileExists(int id)
		{
			return _context.DrawingFile.Any(e => e.Id == id);
		}
	}
}
