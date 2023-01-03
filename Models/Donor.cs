using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BDMS.Models
{
    public class Donor
    {
        public int Id { get; set; }
        [Required]
        public string? Cnic { get; set; }
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }
        [Required]
        public string? Address { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }

        [ForeignKey("Area")]
        public int AreaCode { get; set; }
        public Area Area { get; set; }

        public ICollection<Slot> Slots { get; set; }

    }
}
