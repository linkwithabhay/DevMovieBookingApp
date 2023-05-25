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
            SeedRole(scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>());
            SeedStudents(scope.ServiceProvider.GetRequiredService<IStudentService>());
            SeedAdmin(scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>());
        }
        public static void SeedRole(RoleManager<AppRole> roleManager)
        {
            if (roleManager.Roles.Any()) return;
            roleManager.CreateAsync(new AppRole { Name = "Admin" }).GetAwaiter().GetResult();
        }

        public static void SeedAdmin(UserManager<AppUser> userManager)
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

        public static void SeedStudents(IStudentService repository)
        {
            if (repository.Get().Any()) return;
            var students = new List<Student>
            {
                new Student
                {
                    Name = "Student One",
                    Gender = "M",
                    Age = 20,
                    IsGraduated = false,
                    Courses = new[] { ".Net", "SQL" },
                },
                new Student
                {
                    Name = "Student Two",
                    Gender = "F",
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
