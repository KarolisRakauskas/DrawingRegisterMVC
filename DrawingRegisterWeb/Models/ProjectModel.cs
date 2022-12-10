using DrawingRegisterWeb.Class;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DrawingRegisterWeb.Models
{
	public class ProjectModel
	{
		[Key]
		public int Id { get; set; }
		[Required]
		[DisplayName("Project Number")]
		public string ProjcetNubmer { get; set; } = null!;
		[Required]
		public string Name { get; set; } = null!;
		[DisplayName("Create Date")]
		public DateTime CreateDate { get; set; } = DateTime.Now;
		[Required]
		[DisplayName("Deadline Date")]
		public DateTime DeadlineDate { get; set; }
	}
}
