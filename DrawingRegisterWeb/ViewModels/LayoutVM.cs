using DrawingRegisterWeb.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DrawingRegisterWeb.ViewModels
{
	public class LayoutVM
	{
		public Layout? Layout { get; set; }
		public List<Layout>? Layouts { get; set; }
		public SelectList? ProjectSelectList { get; set; }
		public string? Search { get; set; }
		public string? Projects { get; set; }
	}
}
