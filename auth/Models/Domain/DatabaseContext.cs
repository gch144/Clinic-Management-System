using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace auth.Models.Domain
{
    public class DatabaseContext : IdentityDbContext<User>
    {
        public DbSet<DoctorSchedule> DoctorSchedules { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<DoctorSchedule>()
                .HasOne(ds => ds.Doctor)
                .WithMany(u => u.DoctorSchedules)
                .HasForeignKey(ds => ds.DoctorId);

            modelBuilder.Entity<DoctorSchedule>()
                    .HasMany(ds => ds.Appointments)
                    .WithOne(a => a.DoctorSchedule)
                    .HasForeignKey(a => a.DoctorScheduleId)
                    .OnDelete(DeleteBehavior.Cascade);


        }
    }
}