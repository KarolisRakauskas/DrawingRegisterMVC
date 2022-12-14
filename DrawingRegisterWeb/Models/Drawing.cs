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
		[DisplayName("Drawing Number")]
		public string? DrawingNumber { get; set; }
		[Required]
		[DataType(DataType.Date)]
		[DisplayName("Create Date")]
		public DateTime CreateDate { get; set; } = DateTime.Now;
		public string? Revision { get; set; }
		[Required]
		public string? FileUrl { get; set; }
		[Required]
		[ValidateNever]
		public int ProjectItemId { get; set; }
		[Required]
		[ValidateNever]
		[ForeignKey("ProjectItemId")]
		public ProjectItem? ProjectItem { get; set; }
	}
}
