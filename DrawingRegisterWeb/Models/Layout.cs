using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DrawingRegisterWeb.Models
{
	public class Layout
	{
		[Key]
		public int Id { get; set; }
		[Required]
		[DisplayName("File")]
		public string FileUrl { get; set; } = null!;
		[Required]
		[DisplayName("File Name")]
		public string FileName { get; set; } = null!;
		[Required]
		[DisplayName("File Type")]
		public string FileType { get; set; } = null!;
		public string? Revision { get; set; }
		[Required]
		[DataType(DataType.Date)]
		[DisplayName("Create Date")]
		public DateTime CreateDate { get; set; } = DateTime.Now;
		[Required]
		[DisplayName("Project Id")]
		public int ProjectId { get; set; }
		[ForeignKey("ProjectId")]
		[ValidateNever]
		public Project Project { get; set; } = null!;
	}
}
