using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using TaskManagerAPI.Data;
using TaskManagerAPI.Models;
using TaskManagerAPI.Services;

namespace TaskManagerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JwtService _jwtService;

        public AuthController(AppDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                // Email kontrolü
                if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                {
                    return BadRequest(new { message = "Bu email adresi zaten kullanılıyor!" });
                }

                // Şirket var mı kontrol et
                var company = await _context.Companies
                    .FirstOrDefaultAsync(c => c.Id == request.CompanyId);

                if (company == null)
                {
                    return BadRequest(new { message = "Seçilen şirket bulunamadı!" });
                }

                // Ekip kontrolü - aynı ekipte 6 kişi var mı?
                var teamUsers = await _context.Users
                    .Where(u => u.CompanyId == request.CompanyId && u.TeamName == request.TeamName && u.IsActive)
                    .CountAsync();

                if (teamUsers >= 6)
                {
                    return BadRequest(new { message = $"'{request.TeamName}' ekibi zaten 6 kişi dolu! Başka bir ekip seçin." });
                }

                // Kullanıcı oluştur
                var user = new User
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    CompanyId = company.Id,
                    TeamName = request.TeamName,
                    CreatedDate = DateTime.Now,
                    IsActive = true
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Token oluştur
                var token = _jwtService.GenerateToken(user);

                return Ok(new
                {
                    message = "Kayıt başarılı!",
                    token = token,
                    user = new
                    {
                        id = user.Id,
                        firstName = user.FirstName,
                        lastName = user.LastName,
                        email = user.Email,
                        companyId = user.CompanyId,
                        companyName = company.Name
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Bir hata oluştu: " + ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.Company)
                    .FirstOrDefaultAsync(u => u.Email == request.Email);

                if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                {
                    return BadRequest(new { message = "Email veya şifre hatalı!" });
                }

                var token = _jwtService.GenerateToken(user);

                return Ok(new
                {
                    message = "Giriş başarılı!",
                    token = token,
                    user = new
                    {
                        id = user.Id,
                        firstName = user.FirstName,
                        lastName = user.LastName,
                        email = user.Email,
                        companyId = user.CompanyId,
                        companyName = user.Company?.Name
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Bir hata oluştu: " + ex.Message });
            }
        }

        [HttpGet("companies")]
        public async Task<IActionResult> GetCompanies()
        {
            try
            {
                var companies = await _context.Companies
                    .Where(c => c.IsActive)
                    .OrderBy(c => c.Name)
                    .Select(c => new
                    {
                        id = c.Id,
                        name = c.Name
                    })
                    .ToListAsync();

                return Ok(companies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Şirketler getirilirken hata: " + ex.Message });
            }
        }
    }

    public class RegisterRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int CompanyId { get; set; }
        public string TeamName { get; set; } = string.Empty;
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
