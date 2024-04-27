using EntranceService.DAL.Entity;
using Microsoft.EntityFrameworkCore;

namespace EntranceService.DAL
{
    public class EntranceDbContext: DbContext
    {
        public EntranceDbContext(DbContextOptions<EntranceDbContext> options) : base(options) { }
        public DbSet<Application> Applications { get; set; }
        public DbSet<PassportData> PassportsData { get; set; }
        public DbSet<EducationDocumentData> EducationDocumentsData { get; set; }
        public DbSet<ApplicationPrograms> ApplicationsPrograms { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationPrograms>()
                .HasKey(ap => new { ap.ApplicationId, ap.ProgramId });

            modelBuilder.Entity<EducationDocumentData>()
                .HasKey(ed => new { ed.OwnerId, ed.EducationDocumentId });
        }

    }
}
