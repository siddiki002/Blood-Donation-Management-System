using BDMS.Models;
using Microsoft.EntityFrameworkCore;

namespace BDMS.Data
{
    public class ApplicationDbContext : DbContext 
    {

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Area> Areas { get; set; }
        public DbSet<BloodBag> BloodBags { get; set; }
        public DbSet<BloodCamp> BloodCamps { get; set; }
        public DbSet<Disease> Diseases { get; set; }
        public DbSet<Donor> Donors { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Slot> Slots { get; set; }
        public DbSet<TestedBags> TestedBags { get; set; }

    }
}
