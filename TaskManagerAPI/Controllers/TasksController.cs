using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Data;
using TaskManagerAPI.Models;
using TaskManagerAPI.Services;
using System.Globalization;

namespace TaskManagerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly TaskService _taskService;

        public TasksController(AppDbContext context, TaskService taskService)
        {
            _context = context;
            _taskService = taskService;
        }

        [HttpGet("weekly/{companyId}")]
        public async Task<IActionResult> GetWeeklyTasks(int companyId, [FromQuery] DateTime? date = null)
        {
            try
            {
                var targetDate = date ?? DateTime.Now;
                var monday = GetMondayOfWeek(targetDate);
                var weekNumber = GetWeekNumber(monday);
                var year = monday.Year;

                var tasks = await _context.Tasks
                    .Include(t => t.User)
                    .Include(t => t.TaskType)
                    .Where(t => t.User.CompanyId == companyId && 
                               t.WeekNumber == weekNumber && 
                               t.Year == year)
                    .OrderBy(t => t.AssignedDate)
                    .ThenBy(t => t.User.TeamName)
                    .ThenBy(t => t.TaskType.DifficultyLevel)
                    .ToListAsync();

                Console.WriteLine($"CompanyId: {companyId}, WeekNumber: {weekNumber}, Year: {year}, Tasks Count: {tasks.Count}");

                var result = new
                {
                    weekNumber = weekNumber,
                    year = year,
                    weekStart = monday.ToString("yyyy-MM-dd"),
                    weekEnd = monday.AddDays(6).ToString("yyyy-MM-dd"),
                    tasks = tasks.Select(t => new
                    {
                        id = t.Id,
                        title = t.Title,
                        description = t.Description,
                        status = t.Status.ToString(),
                        assignedDate = t.AssignedDate.ToString("yyyy-MM-dd"),
                        dueDate = t.DueDate?.ToString("yyyy-MM-dd"),
                        difficultyLevel = t.TaskType?.DifficultyLevel ?? 0,
                        user = new
                        {
                            id = t.User.Id,
                            name = $"{t.User.FirstName} {t.User.LastName}",
                            email = t.User.Email,
                            teamName = t.User.TeamName
                        },
                        taskType = new
                        {
                            id = t.TaskType?.Id ?? 0,
                            name = t.TaskType?.Name ?? "",
                            description = t.TaskType?.Description ?? ""
                        }
                    }).ToList()
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Görevler getirilirken hata: " + ex.Message });
            }
        }

        [HttpPost("generate/{companyId}")]
        public async Task<IActionResult> GenerateWeeklyTasks(int companyId, [FromQuery] DateTime? date = null)
        {
            try
            {
                var targetDate = date ?? DateTime.Now;
                var monday = GetMondayOfWeek(targetDate);
                
                // Pazartesi kontrolü kaldırıldı - her zaman görev oluşturulabilir
                var tasks = await _taskService.GenerateWeeklyTasks(companyId, monday);

                return Ok(new
                {
                    message = "Haftalık görevler başarıyla oluşturuldu!",
                    totalTasks = tasks.Count,
                    weekNumber = GetWeekNumber(monday),
                    year = monday.Year
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{taskId}/status")]
        public async Task<IActionResult> UpdateTaskStatus(int taskId, [FromBody] UpdateStatusRequest request)
        {
            try
            {
                var task = await _context.Tasks
                    .Include(t => t.User)
                    .FirstOrDefaultAsync(t => t.Id == taskId);

                if (task == null)
                    return NotFound(new { message = "Görev bulunamadı!" });

                if (!Enum.TryParse<TaskItemStatus>(request.Status, out var status))
                    return BadRequest(new { message = "Geçersiz durum!" });

                task.Status = status;
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Görev durumu başarıyla güncellendi!",
                    taskId = taskId,
                    newStatus = status.ToString(),
                    userName = $"{task.User.FirstName} {task.User.LastName}"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Durum güncellenirken hata: " + ex.Message });
            }
        }

        [HttpGet("statistics/{companyId}")]
        public async Task<IActionResult> GetTaskStatistics(int companyId, [FromQuery] DateTime? date = null)
        {
            try
            {
                var targetDate = date ?? DateTime.Now;
                var monday = GetMondayOfWeek(targetDate);
                var weekNumber = GetWeekNumber(monday);
                var year = monday.Year;

                var tasks = await _context.Tasks
                    .Include(t => t.User)
                    .Where(t => t.User.CompanyId == companyId && 
                               t.WeekNumber == weekNumber && 
                               t.Year == year)
                    .ToListAsync();

                var stats = new
                {
                    total = tasks.Count,
                    completed = tasks.Count(t => t.Status == TaskItemStatus.Completed),
                    inProgress = tasks.Count(t => t.Status == TaskItemStatus.InProgress),
                    notStarted = tasks.Count(t => t.Status == TaskItemStatus.NotStarted)
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "İstatistikler getirilirken hata: " + ex.Message });
            }
        }

        private static DateTime GetMondayOfWeek(DateTime date)
        {
            int daysFromMonday = ((int)date.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
            return date.AddDays(-daysFromMonday).Date;
        }

        private static int GetWeekNumber(DateTime date)
        {
            var calendar = new GregorianCalendar();
            return calendar.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
        }
    }

    public class UpdateStatusRequest
    {
        public string Status { get; set; } = string.Empty;
    }
}
