using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BDMS.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Cnic { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

        [ForeignKey("Organization")]
        public int OrgCode { get; set; }
        public Organization Organization { get; set; }

        [ForeignKey("Area")]
        public int AreaCode { get; set; }
        public Area Area { get; set; }

    }
}
