namespace DrawingRegisterWeb.Models;

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ProjectState
{
	[Key]
	public int Id { get; set; }
	[StringLength(20, MinimumLength = 3, ErrorMessage = "The Name must be a text with a minimum of 3 and a maximum of 20 characters.")]
	[Required]
	public string Name { get; set; } = null!;
	[MinLength(3, ErrorMessage = "The Name must be a text with a minimum of 3 characters.")]
	[Required]
	public string Description { get; set; } = null!;
	[Required]
	public int DrawingRegisterId { get; set; }
	[ForeignKey("DrawingRegisterId")]
	[ValidateNever]
	public DrawingRegister? DrawingRegister { get; set; }
}

