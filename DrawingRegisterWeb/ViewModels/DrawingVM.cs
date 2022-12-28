using DrawingRegisterWeb.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DrawingRegisterWeb.ViewModels
{
	public class DrawingVM
	{
		public Drawing? Drawing { get; set; }
		public List<Drawing>? Drawings { get; set; }
		public SelectList? ProjectSelectList { get; set; }
		public List<DrawingFile>? DrawingFiles { get; set; }
		public string? Search { get; set; }
		public string? Projects { get; set; }
	}
}
