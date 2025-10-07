using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPI.Models
{
    public class Company
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
        
        public List<User> Users { get; set; } = new List<User>();
    }
}
