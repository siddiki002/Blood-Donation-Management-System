using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BDMS.Models
{
    public class BloodBag
    {

        public int Id { get; set; }

        [Required]
        public string BloodGrp { get; set; }

        [ForeignKey("Slot")]
        public int History { get; set; }
        public Slot Slot { get; set; }

        public ICollection<TestedBags> TestedBags { get; set; }

    }
}
