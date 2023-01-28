using DrawingRegisterWeb.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DrawingRegisterWeb.ViewModels
{
	public class RegisterVM
	{
		public DrawingRegister? DrawingRegister { get; set; }
		public List<DrawingRegisterUsers>? DrawingRegisterUsers { get; set; }

	}
}
