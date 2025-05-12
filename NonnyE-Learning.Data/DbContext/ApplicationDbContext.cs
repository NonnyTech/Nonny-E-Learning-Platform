using Microsoft.AspNetCore.Identity;
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
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
	{

		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options) 

        {    
        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollements { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Module> Modules { get; set; }
		public DbSet<ModuleProgress> ModuleProgress { get; set; }
		public DbSet<QuizQuestion> QuizQuestions { get; set; }

	}
}
