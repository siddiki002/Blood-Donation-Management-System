using System.ComponentModel.DataAnnotations;

namespace BDMS.Models
{
    public class Area
    {
        public int Id { get; set; }
        [Required] 
        public string Name { get; set; }
        [Required]
        public string? City { get; set; }
        [Required]
        public string? Province { get; set; }

        public ICollection<BloodCamp> BloodCamps { get; set; }
        public ICollection<Employee> Employees { get; set; }
        public ICollection<Donor> Donors { get; set; }

    }
}
