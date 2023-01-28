using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DrawingRegisterWeb.Models
{
    public class DrawingRegisterInvitation
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int DrawingRegisterId { get; set; }
        [ForeignKey("DrawingRegisterId")]
        [ValidateNever]
        public DrawingRegister? DrawingRegister { get; set; }
        [Required]
        public int StatusId { get; set; }
        [ForeignKey("StatusId")]
        [ValidateNever]
        public Status? Status { get; set; }
    }
}
