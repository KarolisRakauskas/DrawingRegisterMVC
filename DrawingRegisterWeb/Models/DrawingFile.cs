﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DrawingRegisterWeb.Models
{
	public class DrawingFile
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public string? FileUrl { get; set; } = null!;

		[DisplayName("File Name")]
		public string FileName { get; set; }
		[DisplayName("File Type")]
		public string? FileType { get; set; }
		public string? Revision { get; set; }
		[Required]
		[DataType(DataType.Date)]
		[DisplayName("Create Date")]
		public DateTime CreateDate { get; set; } = DateTime.Now;
		[Required]
		[ValidateNever]
		public int DrawingId { get; set; }
		[Required]
		[ValidateNever]
		[ForeignKey("DrawingId")]
		public Drawing Drawing { get; set; } = null!;
	}
}
