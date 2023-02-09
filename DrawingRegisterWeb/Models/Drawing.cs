using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DrawingRegisterWeb.Models
{
	public class Drawing
	{
		[Key]
		public int Id { get; set; }
		[Required]
		[DisplayName("Main assembly drawing number")]
		[MinLength(3, ErrorMessage = "The Drawing Number must be a text with a minimum of 3 characters.")]
		public string DrawingNumber { get; set; } = null!;
		[Required]
		[MinLength(3, ErrorMessage = "The Name must be a text with a minimum of 3 characters.")]
		public string Name { get; set; } = null!;
		[DisplayName("Drawing Description")]
		public string? Description { get; set; }
		[DisplayName("Create Date")]
		[DataType(DataType.Date)]
		public DateTime CreateDate { get; set; } = DateTime.Now;
		[Required]
		[ValidateNever]
		public int ProjectId { get; set; }
		[ForeignKey("ProjectId")]
		[ValidateNever]
		public Project Project { get; set; } = null!;
	}
}
