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
	// Project - Holds main data about user project. Seperates and group drawings, documentation.
	//			 Creates the relationship for drawings, documentation and layouts to ProjectStates.

	[Authorize(Roles = $"{ConstData.Role_Admin_Name},{ConstData.Role_Engr_Name}")]
	public class Model3DController : Controller
	{
		private readonly DrawingRegisterContext _context;
		private readonly IWebHostEnvironment _hostEnvironment;
		private readonly UserManager<IdentityUser> _userManager;

		public Model3DController(
			DrawingRegisterContext context,
			IWebHostEnvironment hostEnvironment,
			UserManager<IdentityUser> userManager)
		{
			_context = context;
			_hostEnvironment = hostEnvironment;
			_userManager = userManager;
		}




		// Upload 3D model
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = $"{ConstData.Role_Admin_Name},{ConstData.Role_Engr_Name}")]
		public async Task<IActionResult> Create(int ProjectId, IFormFile file)
		{
			var user = await _userManager.GetUserAsync(User);
			var drawingRegisterUser = await _context.DrawingRegisterUsers.FirstOrDefaultAsync(dr => dr.UserId == user.Id);
			var project = await _context.Project
				.Include(p => p.ProjectState)
				.Where(p => p.ProjectState!.DrawingRegisterId == drawingRegisterUser!.DrawingRegisterId)
				.FirstOrDefaultAsync(p => p.Id == ProjectId);

			if (project == null && file == null)
			{
				return NotFound();
			}

			//Create File patch and guid name
			string wwwRootPath = _hostEnvironment.WebRootPath;
			string fileName = Guid.NewGuid().ToString();
			var uploads = Path.Combine(wwwRootPath, @"Files\3DModels");
			var extension = Path.GetExtension(file.FileName)!.ToLower();

			//Check if file extension is html
			if (extension != ".html")
			{
				TempData["html"] = "File must be eDrawings Web HTML (*. html)";

				return RedirectToAction("Details", "Projects", new { id = project!.Id });
			}

			// Delete old File
			if (project!.ModelUrl != null)
			{
				var oldFilePath = Path.Combine(_hostEnvironment.WebRootPath, project.ModelUrl!.TrimStart('\\'));

				if (System.IO.File.Exists(oldFilePath) && !oldFilePath.Contains("SeededData"))
				{
					System.IO.File.Delete(oldFilePath);
				}
			}

			// Copy file to wwwroot
			using (var fileStream = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
			{
				await file.CopyToAsync(fileStream);
			}

			project.ModelUrl = @"\Files\3DModels\" + fileName + extension;

			_context.Update(project);
			await _context.SaveChangesAsync();

			return RedirectToAction("Details", "Projects", new { id = project!.Id });
		}




		// Delete 3D model
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = $"{ConstData.Role_Admin_Name},{ConstData.Role_Engr_Name}")]
		public async Task<IActionResult> Delete(int id)
		{
			var user = await _userManager.GetUserAsync(User);
			var drawingRegisterUser = await _context.DrawingRegisterUsers.FirstOrDefaultAsync(dr => dr.UserId == user.Id);
			var project = await _context.Project
				.Include(p => p.ProjectState)
				.Where(p => p.ProjectState!.DrawingRegisterId == drawingRegisterUser!.DrawingRegisterId)
				.FirstOrDefaultAsync(p => p.Id == id);

			if (project == null)
			{
				return NotFound();
			}

			// Delete old File
			var oldFilePath = Path.Combine(_hostEnvironment.WebRootPath, project.ModelUrl!.TrimStart('\\'));

			if (System.IO.File.Exists(oldFilePath) && !oldFilePath.Contains("SeededData"))
			{
				System.IO.File.Delete(oldFilePath);
			}

			project.ModelUrl = null;

			_context.Update(project);
			await _context.SaveChangesAsync();

			return RedirectToAction("Details", "Projects", new { id = project!.Id });
		}
	}
}
