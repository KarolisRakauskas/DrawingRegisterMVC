using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DrawingRegisterWeb.Models
{
	public class DrawingRegisterUsers
	{
		[Key]
		public int Id { get; set; }
		[Required] 
		public int DrawingRegisterId { get; set; }
		[ForeignKey("DrawingRegisterId")]
		[ValidateNever]
		public DrawingRegister DrawingRegister { get; set; } = null!;
		[Required]
		public string UserId { get; set; } = null!;
		[ForeignKey("UserId")]
		[ValidateNever]
		public IdentityUser IdentityUser { get; set; } = null!;
		[Required]
		public string Role { get; set; } = null!;
	}
}
