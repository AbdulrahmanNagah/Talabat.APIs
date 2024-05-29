using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Identity;

namespace Talabat.Repository.Identity
{
    public static class AppIdentityDbContextSeed
    {
        public static async Task SeedUsersAsync(UserManager<AppUser> userManager)
        {
            if (userManager.Users.Count() == 0)
            {
                var user = new AppUser()
                {
                    DisplayName = "Abdulrahman Nagah",
                    Email = "abdulrahman.nagah1@gmail.com",
                    UserName = "abdulrahman.nagah",
                    PhoneNumber = "01122334455"
                };
                await userManager.CreateAsync(user, "Pa$$w0rd");

            }

        }
    }
}
