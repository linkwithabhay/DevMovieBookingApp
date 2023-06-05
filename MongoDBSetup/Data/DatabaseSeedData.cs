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
            var dbContext = scope.ServiceProvider.GetRequiredService<IDbContext>();
            var courses = SeedCourses(dbContext);
            var genders = SeedGenders(dbContext);
            SeedRole(scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>());
            SeedStudents(dbContext, genders, courses);
            SeedAdmin(scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>());
        }

        private static IEnumerable<Course> SeedCourses(IDbContext dbContext)
        {
            var courses = dbContext.Courses.FindAsync(_ => true).GetAwaiter().GetResult().ToList();
            if (courses.Any()) return courses;
            courses = new List<Course>
            {
                new Course { Name = "C#", Rating = 5 },
                new Course { Name = "TypeScript", Rating = 5 },
                new Course { Name = "JavaScript", Rating = 5 },
                new Course { Name = "Python", Rating = 4 },
                new Course { Name = "PHP", Rating = 2 },
                new Course { Name = "C", Rating = 2 },
                new Course { Name = "Java", Rating = 1 },
            };
            dbContext.Courses.InsertManyAsync(courses).GetAwaiter().GetResult();
            return courses;
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

        private static void SeedStudents(IDbContext dbContext, IEnumerable<Gender> genders, IEnumerable<Course> courses)
        {
            if (dbContext.Students.FindAsync(_ => true).GetAwaiter().GetResult().Any()) return;
            var students = new List<Student>
            {
                new Student
                {
                    Name = "Student One",
                    Gender = genders.Where(x => x.Name.Equals("Male")).Select(x => x.Id.ToString()).First(),
                    Age = 20,
                    IsGraduated = false,
                    Courses = courses.Take(4).Select(x => x.Id.ToString()).ToArray(),
                },
                new Student
                {
                    Name = "Student Two",
                    Gender = genders.Where(x => x.Name.Equals("Female")).Select(x => x.Id.ToString()).First(),
                    Age = 19,
                    IsGraduated = false,
                    Courses = courses.TakeLast(4).Select(x => x.Id.ToString()).ToArray(),
                },
            };
            dbContext.Students.InsertManyAsync(students).GetAwaiter().GetResult();
        }
    }
}
