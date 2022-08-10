using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SSOService.Models.Domains;
using SSOService.Models.DTOs.Audit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSOService.Models.DbContexts
{
    public class SSODbContext : DbContext
    {

        public SSODbContext(DbContextOptions<SSODbContext> options) : base(options)
        {
        }
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Application> Applications { get; set; }
        public virtual DbSet<Permission> Permissions { get; set; }
        public virtual DbSet<UserPermission> UserPermissions { get; set; }
        public virtual DbSet<ApplicationPermission> ApplicationPermissions { get; set; }
        public virtual DbSet<UserLogin> UserLogins { get; set; }
        public virtual DbSet<UserDevice> UserDevices { get; set; }
        public virtual DbSet<Subscription> Subscriptions { get; set; }
        public virtual DbSet<ClientSubscription> ClientSubscriptions { get; set; }
        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<SubscriptionServices> SubscriptionServices { get; set; }
        public virtual DbSet<ApplicationAuthentification> ApplicationAuthentifications { get; set; }
        public virtual DbSet<Audit> AuditLogs { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<RoleClaim> RoleClaims { get; set; }
        public virtual DbSet<RolePermission> RolePermissions { get; set; }
        public virtual DbSet<UserClient> UserClients { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<ServiceType> ServiceTypes { get; set; }


        public virtual async Task<int> SaveAndAuditChangesAsync(Guid? userId = null)
        {
            int result = 0;
            try
            {
                OnBeforeSaveChanges(userId);
                result = await base.SaveChangesAsync();
                return result;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return result;
            }
        }

        private void OnBeforeSaveChanges(Guid? userId)
        {
            ChangeTracker.DetectChanges();
            var auditEntries = new List<AuditEntry>();
            foreach (var entry in ChangeTracker.Entries())
            {
                if (!(entry.Entity is Audit || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged))
                {
                    var auditEntry = new AuditEntry
                    {
                        TableName = entry.Entity.GetType().Name,
                        UserId = userId ?? Guid.Empty
                    };
                    auditEntries.Add(auditEntry);
                    foreach (var property in entry.Properties)
                    {
                        string propertyName = property.Metadata.Name;
                        if (property.Metadata.IsPrimaryKey())
                        {
                            auditEntry.KeyValues[propertyName] = property.CurrentValue;
                            continue;
                        }

                        switch (entry.State)
                        {
                            case EntityState.Added:
                                auditEntry.AuditType = Enums.AuditType.Create;
                                auditEntry.NewValues[propertyName] = property.CurrentValue;
                                break;

                            case EntityState.Deleted:
                                auditEntry.AuditType = Enums.AuditType.Delete;
                                auditEntry.OldValues[propertyName] = property.OriginalValue;
                                break;

                            case EntityState.Modified:
                                if (property.IsModified)
                                {
                                    auditEntry.ChangedColumns.Add(propertyName);
                                    auditEntry.AuditType = Enums.AuditType.Update;
                                    auditEntry.OldValues[propertyName] = property.OriginalValue;
                                    auditEntry.NewValues[propertyName] = property.CurrentValue;
                                }
                                break;
                        }
                    }
                }


            }
            foreach (var auditEntry in auditEntries)
            {

                AuditLogs.Add(
                           new Audit
                           {
                               UserId = auditEntry.UserId,
                               Type = auditEntry.AuditType,
                               TableName = auditEntry.TableName,
                               CreatedOn = DateTime.UtcNow,
                               PrimaryKey = JsonConvert.SerializeObject(auditEntry.KeyValues),
                               OldValues = auditEntry.OldValues.Count == 0 ? null : JsonConvert.SerializeObject(auditEntry.OldValues),
                               NewValues = auditEntry.NewValues.Count == 0 ? null : JsonConvert.SerializeObject(auditEntry.NewValues),
                               AffectedColumns = auditEntry.ChangedColumns.Count == 0 ? null : JsonConvert.SerializeObject(auditEntry.ChangedColumns)
                           }

                );
            }
        }

    }
}


