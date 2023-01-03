using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BDMS.Models
{
    public class Slot
    {
        public int Id { get; set; }
        [Required]
        public DateTime Time { get; set; }
        [Required]
        public DateTime Date { get; set; }
        public int bedno { get; set; }
        [Required]
        public String CanDonate { get; set; }
        [Required]
        public String Reject { get; set; }

        [ForeignKey("BloodCamp")]
        public int CampId { get; set; }
        public BloodCamp BloodCamp { get; set; }

        [ForeignKey("Donor")]
        public int DonorId { get; set; }
        public Donor Donor { get; set; }

        public ICollection<BloodBag> BloodBags { get; set; }
    }
}
