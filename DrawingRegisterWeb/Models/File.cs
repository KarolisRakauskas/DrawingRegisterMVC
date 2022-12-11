using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DrawingRegisterWeb.Models
{
    public class File
    {
        [Key]
        public int Id { get; set; }
        public string? FileName { get; set; }
        public string? FileExtension { get; set; }
        [Required]
        [DisplayName("File")]
        public string? FileUrl { get; set; }
        public string? Revision { get; set; }
    }
}
