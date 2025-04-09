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
			string[] roleNames = { "SuperAdmin", "Student" };
			IdentityResult roleResult;

			foreach (var roleName in roleNames)
			{
				var roleExist = await roleManager.RoleExistsAsync(roleName);
				if (!roleExist)
				{
					// Create roles if they do not exist
					roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
				}
			}
			var superAdmins = new List<ApplicationUser>
			 {
				 new ApplicationUser
					 {
						 UserName = "superadmin@nonnyplus.com",
						 Email = "superadmin@nonnyplus.com",
						 EmailConfirmed = true,
						 FirstName="Super",
						 LastName="Admin"

					 },
			  };
			string userPassword = "SuperAdmin@123";

			foreach (var superAdmin in superAdmins)
			{
				var user = await userManager.FindByEmailAsync(superAdmin.Email);

				if (user == null)
				{
					var createPowerUser = await userManager.CreateAsync(superAdmin, userPassword);
					if (createPowerUser.Succeeded)
					{
						await userManager.AddToRoleAsync(superAdmin, "SuperAdmin");
					}
				}
			}


		}


	}

}
