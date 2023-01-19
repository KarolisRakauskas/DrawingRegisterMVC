using System.ComponentModel.DataAnnotations;

namespace DrawingRegisterWeb.Models
{
    public class Status
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;
    }
}
