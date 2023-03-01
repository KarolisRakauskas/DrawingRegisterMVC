
using Aspose.Pdf.Devices;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Signing;
using Org.BouncyCastle.Tsp;

namespace DrawingRegisterWeb.Models
{
	// Snipitest Strings
	public static class HomeCodeSnipits
	{
		public const string roleCSHTML = "@if (User.IsInRole(ConstData.Role_Admin_Name))\r\n{\r\n	<a asp-action='Invitation'>Send an invitation</a>\r\n}";
		public const string role1Csharp = "[Authorize(Roles = ConstData.Role_Admin_Name)]\r\npublic IActionResult Invitation()\r\n{\r\n	return View();\r\n}";
		public const string role2Csharp = "var user = await _userManager.GetUserAsync(User);\r\nawait _userManager.AddToRoleAsync(user, ConstData.Role_Admin_Name);" +
			"\r\nawait _context.SaveChangesAsync();\r\nawait _signInManager.RefreshSignInAsync(user);";
		public const string role3Csharp = "// Restrict from sending invitations to built in app accounts\r\nif (registerUser!.NormalizedEmail == 'Spectator@mail.com'.ToUpper())\r\n" +
			"{\r\n	ModelState.AddModelError('SpectatorRegister',\r\n		'This user is in spectator drawing register. You cant join spectators register');\r\n}";
		public const string role4Csharp = "[HttpPost]\r\n[ValidateAntiForgeryToken]\r\npublic async Task<IActionResult> Create(int drawingId, List<IFormFile> files)\r\n{\r\n" +
			"	if (ModelState.IsValid && files != null)\r\n	{\r\n		foreach (var file in files)\r\n		{\r\n			// Get new file path, Guid name and extension\r\n" +
			"			string wwwRootPath = _hostEnvironment.WebRootPath;\r\n			string fileName = Guid.NewGuid().ToString();\r\n" +
			"			var uploads = Path.Combine(wwwRootPath, @\"Files\\Drawings\");\r\n			var extension = Path.GetExtension(file.FileName)!.ToLower();\r\n\r\n" +
			"			// Create DrawingFile - Drawing file keeps data about uploaded file\r\n			var drawingFile = new DrawingFile()\r\n			{\r\n" +
			"				DrawingId = drawingId,\r\n				CreateDate = DateTime.Now,\r\n				FileName = Path.GetFileNameWithoutExtension(file.FileName)!,\r\n" +
			"				FileType = Path.GetExtension(file.FileName)!.Remove(0, 1),\r\n				FileUrl = @\"\\Files\\Drawings\\\" + fileName + extension\r\n			};\r\n\r\n" +
			"			using (var fileStream = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))\r\n			{\r\n				file.CopyTo(fileStream);" +
			"\r\n			}\r\n\r\n			_context.Add(drawingFile);\r\n			await _context.SaveChangesAsync();" +
			"\r\n		}\r\n	}\r\n	return RedirectToAction(\"Details\", \"Drawings\", new { id = drawingId });\r\n}";
		public const string role5Csharp =
			"var user = await _userManager.GetUserAsync(User);\r\n" + "var drawingRegisterUser = await _context.DrawingRegisterUsers.FirstOrDefaultAsync(dr => dr.UserId == user.Id);\r\n" +
			"var drawing = await _context.Drawing\r\n" +
			"	.Include(d => d.Project)\r\n" + "	.Include(d => d.Project.ProjectState)\r\n" + "	.Where(d => d.Project.ProjectState.DrawingRegisterId == drawingRegisterUser!.DrawingRegisterId)\r\n" +
			"	.FirstOrDefaultAsync(p => p.Id == id);\r\n\r\n" + "// Check if the drawing exists and ensure that the user accesses only the drawings that belong to his drawing register.\r\n" +
			"if (drawing == null)\r\n" + "{\r\n" + "	return NotFound();\r\n" + "}\r\n";
		public const string role6Csharp = "// Check if file size is less then 10 MB\r\n" + "foreach(var file in files)\r\n" + "{\r\n" + "	if(file.Length > 10 * 1024 * 1024)\r\n" +
			"	{\r\n" + "		TempData[\"Size\"] = $\"The maximum filesize is limited to 10 MB.\";\r\n" + "		return RedirectToAction(\"Details\", \"Drawings\", new { id = drawingId });\r\n" +
			"	}\r\n" + "}\r\n";
		public const string role7Csharp = "using Aspose.Pdf;\r\n" + "using Aspose.Pdf.Devices;\r\n" + "\r\n" + "// If file is pdf type - create files thumbnail\r\n" +
			"if(drawingFile.FileType.ToLower() == \"pdf\")\r\n" + "{\r\n" + "	var pdfDocument = new Document(Path.Combine(uploads, fileName + extension));\r\n" + "	int pageIndex = 1;\r\n" +
			"	var page = pdfDocument.Pages[pageIndex];\r\n" + "\r\n" + "	using FileStream imageStream = new (Path.Combine(uploads, fileName + \".jpg\"), FileMode.Create);\r\n" +
			"	var resolution = new Resolution(300);\r\n" + "	var jpegDevice = new JpegDevice(200, 200, resolution, 300);\r\n" + "\r\n" +
			"	jpegDevice.Process(page, imageStream);\r\n" + "	imageStream.Close();\r\n" +"}";
		public const string role8Csharp = "// if automatic Revision checked - add revision to drawing file from file name\r\n" + "if (automaticRevision)\r\n" +
			"{\r\n" + "	if (file.FileName != null && file.FileName.Contains('_'))\r\n" + "	{\r\n" +
			"		int fileNameEndIndex = drawingFile.FileName.LastIndexOf('_');\r\n" + "		drawingFile.Revision = Path.GetFileNameWithoutExtension(file.FileName)[(fileNameEndIndex + 1)..];\r\n" +
			"	}\r\n" + "}\r\n";
		public const string role2CSHTML = "@if(User.IsInRole(ConstData.Role_Admin_Name))\r\n" + "{\r\n" + "	< a asp-action=\"Edit\" asp-route-id=\"@Model.Project.Id\">Edit</a>\r\n" +
			"	<a asp-action=\"Delete\" asp-route-id=\"@Model.Project.Id\">Delete</a>\r\n" + "}\r\n" + "else if (User.IsInRole(ConstData.Role_Engr_Name) &&\r\n" + "	Model.Project.ProjectState.Name != ConstData.State_Completed &&\r\n" +
			"	Model.Project.ProjectState.Name != ConstData.State_Canceled)\r\n" + "{\r\n" + "	< a asp-action=\"Edit\" asp-route-id=\"@Model.Project.Id\">Edit</a>\r\n" + "}\r\n";
		public const string role9Csharp = "// Make sure that only administrator can edit if ProjectState is Defined, Completed or Canceled\r\n" + "if(drawingRegisterUser.Role != ConstData.Role_Admin_Name)\r\n" +
			"{\r\n" + "	if(projectBeforEdit.ProjectState.Name == ConstData.State_Defined ||\r\n" + "		projectBeforEdit.ProjectState.Name == ConstData.State_Completed ||\r\n" +
			"		projectBeforEdit.ProjectState.Name == ConstData.State_Canceled)\r\n" + "	{\r\n" + "		ModelState.AddModelError(\"NoEdit\",\r\n" + "			$\"Only the administrator has the ability to edit this projcet \" +\r\n" +
			"			$\"if project state is set to {projectBeforEdit.ProjectState.Name}.\");\r\n" + "	}\r\n" + "}\r\n\r\n" + "// Detach EF Core Entity from tracking same primary key value\r\n" +
			"_context.Entry(projectBeforEdit).State = EntityState.Detached;";
	}
}
