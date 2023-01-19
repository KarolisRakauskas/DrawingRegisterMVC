using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DrawingRegisterWeb.Models
{
	public class DrawingRegister
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public string UserId { get; set; } = null!;
		[ForeignKey("UserId")]
		[ValidateNever]
		public IdentityUser? IdentityUser { get; set; }
	}
}
