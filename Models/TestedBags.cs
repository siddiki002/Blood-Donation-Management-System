using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace BDMS.Models
{
    public class TestedBags
    {
        public int Id { get; set; }

        [ForeignKey("BloodBag")]
        public int BagId { get; set; }
        public BloodBag BloodBag { get; set; }

        [ForeignKey("Disease")]
        public int DiseaseId { get; set; }
        public Disease Disease { get; set; }

    }
}
