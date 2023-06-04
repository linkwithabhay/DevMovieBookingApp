using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using MongoDBSetup.Models;
using MongoDBSetup.Services;

namespace MongoDBSetup.Data
{
    public static class DatabaseSeedData
    {
        public static void Seed(IServiceProvider service)
        {
            using var scope = service.CreateScope();
            var genders = SeedGenders(scope.ServiceProvider.GetRequiredService<IDbContext>());
            SeedRole(scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>());
            SeedStudents(scope.ServiceProvider.GetRequiredService<IStudentService>(), genders);
            SeedAdmin(scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>());
        }

        private static IEnumerable<Gender> SeedGenders(IDbContext dbContext)
        {
            var genders = dbContext.Genders.FindAsync(x => true).GetAwaiter().GetResult().ToList();
            if (genders.Any()) return genders;
            genders = new List<Gender>
            {
                new Gender { Name = "Male" },
                new Gender { Name = "Female" }
            };
            dbContext.Genders.InsertManyAsync(genders).GetAwaiter().GetResult();
            return genders;
        }

        private static void SeedRole(RoleManager<AppRole> roleManager)
        {
            if (roleManager.Roles.Any()) return;
            roleManager.CreateAsync(new AppRole { Name = "Admin" }).GetAwaiter().GetResult();
        }

        private static void SeedAdmin(UserManager<AppUser> userManager)
        {
            if (userManager.Users.Any()) return;
            var adminUser = new AppUser
            {
                UserName = "admin",
                Email = "admin@example.com",
            };
            userManager.CreateAsync(adminUser, "Admin@123").GetAwaiter().GetResult();
            userManager.AddToRoleAsync(adminUser, "Admin").GetAwaiter().GetResult();
        }

        private static void SeedStudents(IStudentService repository, IEnumerable<Gender> genders)
        {
            if (repository.Get().Any()) return;
            var students = new List<Student>
            {
                new Student
                {
                    Name = "Student One",
                    Gender = genders.First().Id,
                    Age = 20,
                    IsGraduated = false,
                    Courses = new[] { ".Net", "SQL" },
                },
                new Student
                {
                    Name = "Student Two",
                    Gender = genders.Last().Id,
                    Age = 19,
                    IsGraduated = false,
                    Courses = new[] { ".Net", "SQL" },
                },
            };
            foreach (var student in students)
            {
                repository.Create(student);
            }
        }
    }
}
