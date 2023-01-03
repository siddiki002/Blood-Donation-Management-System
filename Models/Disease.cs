using System.ComponentModel.DataAnnotations;

namespace BDMS.Models
{
    public class Disease
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string RejectBag { get; set; }
        [Required]
        public string RejectDonor { get; set; }

        public ICollection<TestedBags> TestedBags { get; set; }

    }
}
