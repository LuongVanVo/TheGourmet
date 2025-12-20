using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using TheGourmet.Domain.Entities.Identity;

namespace TheGourmet.Infrastructure.Persistence
{
    public class DBSeeder
    {
        public static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager)
        {
            string[] roles = { "Admin", "Customer", };

            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new ApplicationRole 
                    { 
                        Name = roleName
                    });
                    Console.WriteLine($"✅ Role '{roleName}' created successfully!");
                }
                else
                {
                    Console.WriteLine($"ℹ️ Role '{roleName}' already exists.");
                }
            }
        }
    }
}