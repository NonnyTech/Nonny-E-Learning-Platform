using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NonnyE_Learning.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NonnyE_Learning.Data.DbContext
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options) 

        {    
        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollements { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Module> Modules { get; set; }
		public DbSet<ModuleProgress> ModuleProgress { get; set; }


		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.Entity<Course>().HasData
				(

				 new Course
				 {
					 CourseId = 1,
					 Category = "Web Development",
					 Title = "ASP.NET Core MVC",
					 Description = "Build powerful web applications using ASP.NET Core MVC.",
					 ImageUrl = "/upload/xcourse_01.png.pagespeed.ic.XTOvCuUmZu.png",
					 Instructor = "Anthony Ikemefuna",
					 Lectures = 23,
					 Duration = "12 weeks",
					 Price = 400000
				 },
			new Course
			{
				CourseId = 2,
				Category = "Design",
				Title = "Graphics Design with Corel draw.",
				Description = "Master graphic design using Adobe Photoshop and Illustrator.",
				ImageUrl = "/upload/xcourse_02.png.pagespeed.ic.PL7Wu2UcSB.png",
				Instructor = "Rita Ikemefuna",
				Lectures = 23,
				Duration = "18 weeks",
				Price = 250000
			},
			new Course
			{
				CourseId = 3,
				Category = "Programming",
				Title = "C# for Beginners",
				Description = "Learn the fundamentals of C# programming.",
				ImageUrl = "/upload/xcourse_03.png.pagespeed.ic.8e1MyY5M7i.png",
				Instructor = "Stanley Nonso",
				Lectures = 23,
				Duration = "20 weeks",
				Price = 300000
			},
			new Course
			{
				CourseId = 4,
				Category = "Backend Development",
				Title = "Building Web APIs with .NET",
				Description = "Learn how to build scalable Web APIs using ASP.NET Core.",
				ImageUrl = "/upload/xcourse_04.png.pagespeed.ic.2rIKmUwjA7.png",
				Instructor = "Anthony Ikemefuna",
				Lectures = 23,
				Duration = "24 weeks",
				Price = 500000
			}





				);
			modelBuilder.Entity<Module>().HasData(
		new Module
		{
			ModuleId = 1,
			Title = "Introduction to C#",
			CourseContent = "We are one",
			CourseId = 3,
			Order = 1
		},
		new Module
		{
			ModuleId = 2,
			Title = "Getting ready for C#",
			CourseContent = "We are one",
			CourseId = 3,
			Order = 2
		},
		new Module
		{
			ModuleId = 3,
			Title = "The World of Variables and Operators",
			CourseContent = "We are one",
			CourseId = 3,
			Order = 3
		},
		new Module
		{
			ModuleId = 4,
			Title = "Arrays, Strings and Lists",
			CourseContent = "We are one",
			CourseId = 3,
			Order = 4
		},
		new Module
		{
			ModuleId = 5,
			Title = "Making our Program Interactive",
			CourseContent = "We are one",
			CourseId = 3,
			Order = 5
		},
		new Module
		{
			ModuleId = 6,
			Title = "Making Choices and Decisions",
			CourseContent = "We are one",
			CourseId = 3,
			Order = 6
		},
		new Module
		{
			ModuleId = 7,
			Title = "Object-Oriented Programming Part ",
			CourseContent = "We are one",
			CourseId = 3,
			Order = 7
		},
		new Module
		{
			ModuleId = 8,
			Title = "Enum and Struct",
			CourseContent = "We are one",
			CourseId = 3,
			Order = 8
		},
		new Module
		{
			ModuleId = 9,
			Title = "Enum and Struct",
			CourseContent = "We are one",
			CourseId = 3,
			Order = 8
		}
	);



		}





	}
}
