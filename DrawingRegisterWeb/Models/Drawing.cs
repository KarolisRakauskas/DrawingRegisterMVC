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
        public string DrawingNumber { get; set; } = null!;
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public int FileId { get; set; }
        [ForeignKey("FileId")]
        public File File { get; set; } = null!;

    }
}
