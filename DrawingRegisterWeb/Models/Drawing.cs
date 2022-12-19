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
        public string DrawingNumber { get; set; } = null!;
        [Required]
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
