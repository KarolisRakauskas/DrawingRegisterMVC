using System.ComponentModel.DataAnnotations;

namespace DrawingRegisterWeb.Models
{
	public class DrawingRegister
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public string UserId { get; set; } = null!;
	}
}
