using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BDMS.Models
{
    public class Organization
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        public string? Desc { get; set; }
        [Required]
        public string? Password { get; set; }
        [Required]
        public string? Address { get; set; }

        public ICollection<Employee> Employees { get; set; }
        public ICollection<BloodCamp> BloodCamps { get; set; }
    }
}
