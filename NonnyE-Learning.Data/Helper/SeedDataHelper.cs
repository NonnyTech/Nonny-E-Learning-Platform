using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using NonnyE_Learning.Data.DbContext;
using NonnyE_Learning.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonnyE_Learning.Data.Helper
{
	public class SeedDataHelper
	{
		public static async Task SeedAdminDataAsync(IServiceProvider serviceProvider, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
		{
			string[] roleNames = { "SuperAdmin", "Student", "Instructor" };
			IdentityResult roleResult;

			// Create roles if not exist
			foreach (var roleName in roleNames)
			{
				var roleExist = await roleManager.RoleExistsAsync(roleName);
				if (!roleExist)
				{
					roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
				}
			}

			// Seed SuperAdmin
			var superAdmins = new List<ApplicationUser>
		{
			new ApplicationUser
			{   Id = Guid.NewGuid().ToString(),
				UserName = "superadmin@nonnyplus.com",
				Email = "superadmin@nonnyplus.com",
				EmailConfirmed = true,
				FirstName = "Super",
				LastName = "Admin"
			}
		};

			string superAdminPassword = "SuperAdmin@123";

			foreach (var superAdmin in superAdmins)
			{
				var user = await userManager.FindByEmailAsync(superAdmin.Email);
				if (user == null)
				{
					var createUser = await userManager.CreateAsync(superAdmin, superAdminPassword);
					if (createUser.Succeeded)
					{
						await userManager.AddToRoleAsync(superAdmin, "SuperAdmin");
					}
				}
			}

			// Seed Instructors
			var instructors = new List<ApplicationUser>
		{
			new ApplicationUser
			{
				Id = Guid.NewGuid().ToString(),
				UserName = "Instructor1@nonnyplus.com",
				Email = "Instructor1@nonnyplus.com",
				EmailConfirmed = true,
				FirstName = "Stanley",
				LastName = "Nonso"
			},
			new ApplicationUser
			{
				Id = Guid.NewGuid().ToString(),
				UserName = "Instructor2@nonnyplus.com",
				Email = "Instructor2@nonnyplus.com",
				EmailConfirmed = true,
				FirstName = "Anthony",
				LastName = "Ikemefuna"
			},
			new ApplicationUser
			{
				Id = Guid.NewGuid().ToString(),
				UserName = "Instructor3@nonnyplus.com",
				Email = "Instructor3@nonnyplus.com",
				EmailConfirmed = true,
				FirstName = "Rita",
				LastName = "Ikemefuna"
			}
		};

			string instructorPassword = "Instructor@123";

			foreach (var instructor in instructors)
			{
				var user = await userManager.FindByEmailAsync(instructor.Email);
				if (user == null)
				{
					var createInstructor = await userManager.CreateAsync(instructor, instructorPassword);
					if (createInstructor.Succeeded)
					{
						await userManager.AddToRoleAsync(instructor, "Instructor");
					}
				}
			}
		}
	}
}
