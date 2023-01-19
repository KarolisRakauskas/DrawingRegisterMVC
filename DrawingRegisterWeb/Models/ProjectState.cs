namespace DrawingRegisterWeb.Models;

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ProjectState
{
	[Key]
	public int Id { get; set; }
	[Required]
	public string Name { get; set; } = null!;
	[Required]
	public string Description { get; set; } = null!;
	[Required]
	public int DrawingRegisterId { get; set; }
	[ForeignKey("DrawingRegisterId")]
	[ValidateNever]
	public DrawingRegister? DrawingRegister { get; set; }
}

