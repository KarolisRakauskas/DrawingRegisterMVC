using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DrawingRegisterWeb.Models
{
    public class ProjectItem
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [DisplayName("Item Number")]
        public string Number { get; set; } = null!;
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        [DisplayName("Item Description")]
        public string Description { get; set; } = null!;
        [Required]
        [ValidateNever]
        public int ProjectId { get; set; }
        [ForeignKey("ProjectId")]
        [ValidateNever]
        public Project Project { get; set; }
    }
}
