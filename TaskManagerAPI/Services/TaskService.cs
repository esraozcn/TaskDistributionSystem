using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Data;
using TaskManagerAPI.Models;
using System.Globalization;

namespace TaskManagerAPI.Services
{
    public class TaskService
    {
        private readonly AppDbContext _context;

        public TaskService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<TaskItem>> GenerateWeeklyTasks(int companyId, DateTime monday)
        {
            // Şirket kullanıcılarını ekiplere göre al
            var users = await _context.Users
                .Where(u => u.CompanyId == companyId && u.IsActive)
                .OrderBy(u => u.TeamName)
                .ThenBy(u => u.Id)
                .ToListAsync();

            if (users.Count == 0)
            {
                throw new InvalidOperationException("Şirkette aktif kullanıcı bulunamadı!");
            }

            // Ekipleri grupla
            var teams = users.GroupBy(u => u.TeamName).ToList();
            
            // Her ekipte tam 6 kişi olmalı
            foreach (var team in teams)
            {
                if (team.Count() != 6)
                {
                    throw new InvalidOperationException($"Ekip '{team.Key}' tam 6 kişi olmalı! Mevcut: {team.Count()}");
                }
            }

            var weekNumber = GetWeekNumber(monday);
            var year = monday.Year;

            // Bu hafta için zaten görev var mı kontrol et
            var existingTasks = await _context.Tasks
                .Where(t => t.WeekNumber == weekNumber && t.Year == year)
                .AnyAsync();

            if (existingTasks)
            {
                throw new InvalidOperationException($"Bu hafta için zaten görev ataması yapılmış! ({year}-W{weekNumber})");
            }

            // Görev türlerini al
            var taskTypes = await _context.TaskTypes
                .OrderBy(tt => tt.DifficultyLevel)
                .ToListAsync();

            if (taskTypes.Count != 6)
            {
                throw new InvalidOperationException("Tam 6 görev türü olmalı!");
            }

            var tasks = new List<TaskItem>();

            // Her ekip için Sudoku mantığıyla görev dağıtımı
            foreach (var team in teams)
            {
                var teamUsers = team.ToList();
                
                // 6 gün için (Pazartesi-Cumartesi)
                for (int day = 0; day < 6; day++)
                {
                    var currentDate = monday.AddDays(day);
                    
                    // Her gün için 6 görev (1-6 zorluk seviyesi)
                    for (int taskIndex = 0; taskIndex < 6; taskIndex++)
                    {
                        // Sudoku mantığı: (userIndex + day) % 6
                        var userIndex = (taskIndex + day) % 6;
                        var user = teamUsers[userIndex];
                        var taskType = taskTypes[taskIndex];

                        var task = new TaskItem
                        {
                            Title = $"{taskType.Name} - {GetDayName(day)}",
                            Description = taskType.Description,
                            UserId = user.Id,
                            TaskTypeId = taskType.Id,
                            Status = TaskItemStatus.NotStarted,
                            AssignedDate = currentDate,
                            DueDate = currentDate,
                            WeekNumber = weekNumber,
                            Year = year
                        };

                        tasks.Add(task);
                    }
                }
            }

            _context.Tasks.AddRange(tasks);
            await _context.SaveChangesAsync();

            return tasks;
        }

        private static List<(string Title, string Description)> GetTaskTemplates()
        {
            return new List<(string, string)>
            {
                ("Frontend Geliştirme", "React componentleri ve UI tasarımı"),
                ("Backend API", "REST API endpoint'leri ve business logic"),
                ("Veritabanı İşlemleri", "SQL sorguları ve veri modelleme"),
                ("Test Yazma", "Unit test ve integration test"),
                ("Dokümantasyon", "Kod dokümantasyonu ve API dokümantasyonu"),
                ("Code Review", "Kod inceleme ve kalite kontrolü"),
                ("DevOps", "Deployment ve CI/CD işlemleri"),
                ("Proje Yönetimi", "Sprint planlama ve görev takibi")
            };
        }

        private static int GetWeekNumber(DateTime date)
        {
            var calendar = new GregorianCalendar();
            return calendar.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
        }

        private static string GetDayName(int dayIndex)
        {
            var dayNames = new[] { "Pazartesi", "Salı", "Çarşamba", "Perşembe", "Cuma", "Cumartesi" };
            return dayNames[dayIndex];
        }
    }
}
