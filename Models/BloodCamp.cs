using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BDMS.Models
{
    public class BloodCamp
    {
        public int Id { get; set; }

        [ForeignKey("Organization")]
        public int OrgCode{ get; set; }
        public Organization Organization { get; set; }

        [ForeignKey("Area")]
        public int AreaCode { get; set; }
        public Area Area { get; set; }

        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }
        public int beds { get; set; }

        public ICollection<Slot> Slots { get; set; }
    }
}
