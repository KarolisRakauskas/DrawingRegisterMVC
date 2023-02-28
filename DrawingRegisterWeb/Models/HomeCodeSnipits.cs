using Aspose.Pdf.Devices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

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
	}
}
