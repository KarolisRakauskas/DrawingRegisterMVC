using DrawingRegisterWeb.Data;
using DrawingRegisterWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DrawingRegisterWeb.Controllers
{
	public class ProjectsController : Controller
	{
		private readonly DrawingRegisterContext _context;

		public ProjectsController(DrawingRegisterContext context)
		{
			_context = context;
		}

		//GET: Projects
		public async Task<IActionResult> Index()
		{
			return View(await _context.Project.Include(p => p.ProjectState).ToListAsync());
		}
	}
}
