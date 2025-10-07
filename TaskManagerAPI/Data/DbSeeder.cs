using TaskManagerAPI.Models;

namespace TaskManagerAPI.Data
{
    public static class DbSeeder
    {
        public static void SeedData(AppDbContext context)
        {
            // Görev türlerini oluştur
            if (!context.TaskTypes.Any())
            {
                var taskTypes = new List<TaskType>
                {
                    new TaskType { Name = "Frontend Development", DifficultyLevel = 1, Description = "React, Vue, Angular geliştirme" },
                    new TaskType { Name = "Backend API Development", DifficultyLevel = 2, Description = "REST API, GraphQL geliştirme" },
                    new TaskType { Name = "Database Operations", DifficultyLevel = 3, Description = "SQL, NoSQL veritabanı işlemleri" },
                    new TaskType { Name = "Testing & QA", DifficultyLevel = 4, Description = "Unit test, integration test" },
                    new TaskType { Name = "DevOps & Deployment", DifficultyLevel = 5, Description = "CI/CD, Docker, Kubernetes" },
                    new TaskType { Name = "Code Review & Documentation", DifficultyLevel = 6, Description = "Kod inceleme ve dokümantasyon" }
                };

                context.TaskTypes.AddRange(taskTypes);
                context.SaveChanges();
            }

            // Örnek şirketler oluştur
            if (!context.Companies.Any())
            {
                var companies = new List<Company>
                {
                    new Company { Name = "TechSoft A.Ş.", CreatedDate = DateTime.Now, IsActive = true },
                    new Company { Name = "Digital Solutions Ltd.", CreatedDate = DateTime.Now, IsActive = true },
                    new Company { Name = "Innovation Systems", CreatedDate = DateTime.Now, IsActive = true },
                    new Company { Name = "Smart Development Co.", CreatedDate = DateTime.Now, IsActive = true },
                    new Company { Name = "Future Technologies", CreatedDate = DateTime.Now, IsActive = true },
                    new Company { Name = "Cloud Computing Inc.", CreatedDate = DateTime.Now, IsActive = true },
                    new Company { Name = "AI Solutions Group", CreatedDate = DateTime.Now, IsActive = true },
                    new Company { Name = "Data Analytics Pro", CreatedDate = DateTime.Now, IsActive = true },
                    new Company { Name = "Mobile Apps Studio", CreatedDate = DateTime.Now, IsActive = true },
                    new Company { Name = "Web Development Hub", CreatedDate = DateTime.Now, IsActive = true },
                    new Company { Name = "Cyber Security Corp", CreatedDate = DateTime.Now, IsActive = true },
                    new Company { Name = "Blockchain Ventures", CreatedDate = DateTime.Now, IsActive = true },
                    new Company { Name = "IoT Innovations", CreatedDate = DateTime.Now, IsActive = true },
                    new Company { Name = "Machine Learning Co.", CreatedDate = DateTime.Now, IsActive = true },
                    new Company { Name = "DevOps Solutions", CreatedDate = DateTime.Now, IsActive = true }
                };

                context.Companies.AddRange(companies);
                context.SaveChanges();

                // Sadece admin kullanıcı oluştur - diğer kullanıcıları sen dinamik kaydedeceksin
                var firstCompany = companies.First();
                var adminUser = new User
                {
                    FirstName = "Admin",
                    LastName = "User",
                    Email = "admin@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                    CompanyId = firstCompany.Id,
                    TeamName = "Admin Ekibi",
                    CreatedDate = DateTime.Now,
                    IsActive = true
                };

                context.Users.Add(adminUser);
                context.SaveChanges();
            }
        }
    }
}
