namespace DrawingRegisterWeb.Models;
using System.ComponentModel.DataAnnotations;


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
}

