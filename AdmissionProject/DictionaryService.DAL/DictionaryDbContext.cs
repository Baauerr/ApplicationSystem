using DictionaryService.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DictionaryService.DAL
{
    public class DictionaryDbContext : DbContext
    {
        public DictionaryDbContext(DbContextOptions<DictionaryDbContext> options) : base(options) { }
        public DbSet<DocumentType> DocumentTypes { get; set; }
        public DbSet<EducationLevel> EducationLevels { get; set; }
        public DbSet<NextEducationLevel> NextEducationLevelDocuments { get; set; }
        public DbSet<Faculty> Faculties { get; set; }
        public DbSet<Program> Programs { get; set; }
        public DbSet<ImportHistory> ImportHistory { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<NextEducationLevel>()
                .HasKey(e => new { e.EducationLevelId, e.DocumentTypeId });

            modelBuilder.Entity<EducationLevel>()
                .HasKey(e => e.Id);
            modelBuilder.Entity<EducationLevel>()
                .HasIndex(d => d.Id);
            modelBuilder.Entity<Faculty>()
                .HasIndex(d => d.Id);
            modelBuilder.Entity<DocumentType>()
                .HasIndex(d => d.Id);
            modelBuilder.Entity<Program>()
                .HasIndex(d => d.Id);
            modelBuilder.Entity<ImportHistory>()
                .HasIndex(d => d.Id);
        }
    }
}
