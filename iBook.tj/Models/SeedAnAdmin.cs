using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iBook.tj.Db;
using Microsoft.AspNetCore.Identity;
using iBook.tj.Areas.Identity.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace iBook.tj.Models
{
    public class SeedAnAdmin
    {

        private DatabaseContext context;

        public SeedAnAdmin(DatabaseContext _context)
        {
            context = _context;
        }

        public async void Initialize()
        {
            string[] roles = new string[] { "admin", "customer" };
            foreach (var role in roles)
            {
                var roleStore = new RoleStore<IdentityRole>(context);
                if (!context.Roles.Any(r => r.Name == role))
                {
                    await roleStore.CreateAsync(new IdentityRole { Name = role, NormalizedName = role.ToUpper()});
                }
            }

            var user = new iBooktjUser
            {
                FirstName = "Admin",
                LastName = "Admin",
                UserName = "admin@ibook.tj",
                NormalizedUserName = "ADMIN@IBOOK.TJ",
                NormalizedEmail = "ADMIN@IBOOK.TJ",
                Email = "admin@ibook.tj",
                SecurityStamp = Guid.NewGuid().ToString("D")
            };

            if( !context.Users.Any(u=> u.UserName == user.UserName))
            {
                var password = new PasswordHasher<iBooktjUser>();
                var hashed = password.HashPassword(user, "admin");
                user.PasswordHash = hashed;
                var userStore = new UserStore<iBooktjUser>(context);
                await userStore.CreateAsync(user);
                await userStore.AddToRoleAsync(user, "ADMIN");
            }
            await context.SaveChangesAsync();
        }
    }
}
