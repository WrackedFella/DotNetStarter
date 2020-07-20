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
        public MyProjectContext()
        {
        }

        public MyProjectContext(DbContextOptions options)
            : base(options)
        {
        }

        #region Tables

        // People
        public virtual DbSet<Person> Persons { get; set; }

        public virtual DbSet<Address> Addresses { get; set; }

        // System
        public virtual DbSet<AuditHistory> AuditHistory { get; set; }

        #endregion Tables

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=.;Initial Catalog=MyProject;Integrated Security=True");
            }

#if DEBUG
            optionsBuilder.EnableSensitiveDataLogging();
#endif
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            _ = modelBuilder.EnableAutoHistory<AuditHistory>(o =>
            {
                o.LimitChangedLength = false;
                o.RowIdMaxLength = 4000;
            });

            foreach (var et in modelBuilder.Model.GetEntityTypes())
            {
                var uid = et.GetProperties().FirstOrDefault(p => p.IsKey());
                if (uid != null && uid.ClrType == typeof(Guid))
                {
                    uid.SetDefaultValueSql("(newid())");
                }

                foreach (var dateProp in et.GetProperties().Where(p => p.ClrType == typeof(DateTime)))
                {
                    dateProp.SetDefaultValueSql("(getdate())");
                }

                foreach (var decimalProp in et.GetProperties().Where(p => p.ClrType == typeof(decimal)))
                {
                    decimalProp.SetColumnType("decimal(18, 6)");
                }
            }

            #region Person

            modelBuilder.Entity<Address>()
                .ToTable("Addresses", SchemaNames.Person);
            //.HasData(SeedData.SeedData.Addresses);
            modelBuilder.Entity<Person>()
                .ToTable("Person", SchemaNames.Person);
            //.HasData(SeedData.SeedData.Persons);

            #endregion

            #region System

            #endregion

        }

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