using DocumentService.DAL.Entity;
using Microsoft.EntityFrameworkCore;

namespace DocumentService.DAL
{
    public class DocumentDbContext: DbContext
    {
        public DocumentDbContext(DbContextOptions<DocumentDbContext> options) : base(options) { }
        public DbSet<PassportForm> PassportsData { get; set; }
        public DbSet<EducationDocumentForm> EducationDocumentsData { get; set; }
        public DbSet<DocumentFile> PassportsFiles { get; set; }
        public DbSet<DocumentFile> EducationDocumentsFiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EducationDocumentForm>()
                .HasKey(ed => new { ed.OwnerId, ed.EducationLevelId });
        }
    }
}
