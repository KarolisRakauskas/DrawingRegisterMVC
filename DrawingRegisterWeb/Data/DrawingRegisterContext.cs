﻿
using Microsoft.EntityFrameworkCore;
using DrawingRegisterWeb.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace DrawingRegisterWeb.Data
{
	public class ApplicationDbContext : IdentityDbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{
		}

		public DbSet<Project> Project { get; set; }
		public DbSet<ProjectState> ProjectState { get; set; }
		public DbSet<Drawing> Drawing { get; set; }
		public DbSet<DrawingFile> DrawingFile { get; set; }
		public DbSet<Documentation> Documentation { get; set; }
		public DbSet<Layout> Layout { get; set; }
		public DbSet<DrawingRegister> DrawingRegisters { get; set; }
		public DbSet<Status> Statuses { get; set; }
		public DbSet<Invitation> Invitations { get; set; }
		public DbSet<DrawingRegisterUsers> DrawingRegisterUsers { get; set; }

	}
}