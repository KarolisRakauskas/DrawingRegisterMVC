using DrawingRegisterWeb.Models;

namespace DrawingRegisterWeb.ViewModels
{
	public class RegisterVM
	{
		public DrawingRegister? DrawingRegister { get; set; }
		public List<DrawingRegisterUsers>? DrawingRegisterUsers { get; set; }
	}
}
