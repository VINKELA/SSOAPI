using Microsoft.EntityFrameworkCore;
using SSOMachine.Models.Domains;

namespace SSOService.Models.Domains
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions<DbContext> options) : base(options)
        {
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }
        public DbSet<ApplicationPermission> ApplicationPermissions { get; set; }
        public DbSet<UserLogin> UserLogins { get; set; }
        public DbSet<UserDevice> UserDevices { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<ClientSubscription> ClientSubscriptions { get; set; }
        public DbSet<Services> Services { get; set; }
        public DbSet<SubscriptionServices> SubscriptionServices { get; set; }
        public DbSet<ApplicationAuthentification> ApplicationAuthentifications { get; set; }
    }
}
