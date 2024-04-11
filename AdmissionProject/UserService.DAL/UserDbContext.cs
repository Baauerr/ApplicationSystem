using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserService.DAL.Entity;

namespace UserService.DAL
{
    public class UserDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid, IdentityUserClaim<Guid>,
    IdentityUserRole<Guid>, IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }
        public new DbSet<User> Users { get; set; } = null!;
    }
}
