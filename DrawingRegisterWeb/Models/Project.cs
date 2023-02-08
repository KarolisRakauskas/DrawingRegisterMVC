using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DrawingRegisterWeb.Models
{
	public class Project
	{
		[Key]
		public int Id { get; set; }
		[Required]
		[DisplayName("Project Number")]
		[MinLength(3, ErrorMessage = "The Project Number must be a text with a minimum of 3 characters.")]
		public string ProjectNubmer { get; set; } = null!;
		[Required]
		[MinLength(3, ErrorMessage = "The Name must be a text with a minimum of 3 characters.")]
		public string Name { get; set; } = null!;
		[Required]
		[MinLength(3, ErrorMessage = "The Description must be a text with a minimum of 3 characters.")]
		public string Description { get; set; } = null!;
		[DisplayName("Create Date")]
		[DataType(DataType.Date)]
		public DateTime CreateDate { get; set; } = DateTime.Now;
		[Required]
		[DisplayName("Deadline Date")]
		[DataType(DataType.Date)]
		public DateTime DeadlineDate { get; set; }
		[Required]
		public int ProjectStateId { get; set; }
		[ForeignKey("ProjectStateId")]
		[ValidateNever]
		public ProjectState? ProjectState { get; set; }
	}
}
