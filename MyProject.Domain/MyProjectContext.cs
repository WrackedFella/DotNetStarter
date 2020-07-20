using Microsoft.EntityFrameworkCore;
using MyProject.Domain.Core;
using MyProject.Domain.People;
using MyProject.Domain.System;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyProject.Domain
{
    public class MyProjectContext : DbContext
    {
        #region Tables

        // People
        public virtual DbSet<Person> Persons { get; set; }

        public virtual DbSet<Address> Addresses { get; set; }

        // System
        public virtual DbSet<AuditHistory> AuditHistory { get; set; }

        #endregion Tables

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new InvalidOperationException("You must supply a username for audit purposes.");
        }

        public override int SaveChanges()
        {
            throw new InvalidOperationException("You must supply a username for audit purposes.");
        }

        public async Task<int> SaveOnlyMilkPrepLogsAsync(string modifiedBy, CancellationToken cancellationToken = default)
        {
            foreach (var entry in this.ChangeTracker.Entries().Where(x => !(x.Entity is LogMessage)))
            {
                entry.State = EntityState.Unchanged;
            }
            SetAuditDetails(modifiedBy);

            int token = await base.SaveChangesAsync(cancellationToken);
            return token;
        }

        public async Task<int> SaveChangesAsync(string modifiedBy, CancellationToken cancellationToken = default)
        {
            SetAuditDetails(modifiedBy);

            int token = await base.SaveChangesAsync(cancellationToken);
            return token;
        }

        public int SaveChanges(string modifiedBy)
        {
            SetAuditDetails(modifiedBy);
            return base.SaveChanges();
        }

        private void SetAuditDetails(string modifiedBy)
        {
            var statesToTrack = new[]
            {
                EntityState.Added, EntityState.Modified, EntityState.Deleted
            };
            foreach (var dbDomainObject in this.ChangeTracker.Entries<DomainObject>()
                .Where(e => statesToTrack.Contains(e.State)))
            {
                dbDomainObject.Entity.SetAuditDetails(modifiedBy);
            }
            this.EnsureAutoHistory(() => new AuditHistory
            {
                LastModifiedBy = modifiedBy
            });
        }
    }
}