using Microsoft.EntityFrameworkCore;
using NotificationService.DAL.Entity;

namespace NotificationService.DAL
{
    public class NotificationDbContext: DbContext
    {
        public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options) { }
        public DbSet<Notification> Notifications { get; set; }
    }
}
