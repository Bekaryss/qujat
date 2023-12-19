using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Qujat.Core.Data.Entities;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System;
using Qujat.Core.Services;

namespace Qujat.Core.Data
{
    public class ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        TimeProviderService timeProviderService = null) : 
        IdentityDbContext<ApplicationUserEntity, IdentityRole<long>, long>(options)
    {
        private readonly TimeProviderService _timeProviderService = timeProviderService;
        
        public DbSet<CategoryEntity> Categories { get; set; }
        public DbSet<SubcategoryEntity> Subcategories { get; set; }
        public DbSet<DocumentEntity> Documents { get; set; }
        public DbSet<DocumentBlobEntity> DocumentBlobs { get; set; }
        public DbSet<DocumentSubcategoryRelationEntity> DocumentSubcategoryRelations { get; set; }
        public DbSet<BlobEntity> Blobs { get; set; }
        public DbSet<LinkEntity> Links { get; set; }
        public DbSet<IconBlobEntity> Icons { get; set; }
        public DbSet<DocumentActionEventEntity> DocumentActionEventEntities { get; set; }
        public DbSet<ApplicationConfigurationEntity> ApplicationConfiguration { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasDefaultSchema("public");

            builder.Entity<CategoryEntity>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity
                    .HasMany(x => x.Subcategories)
                    .WithOne(x => x.ParentCategory)
                    .HasForeignKey(x => x.ParentCategoryId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .HasOne(x => x.IconBlob)
                    .WithMany(x => x.Categories)
                    .HasForeignKey(x => x.IconBlobId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<SubcategoryEntity>(entity =>
            {
                entity.HasKey(x => x.Id);
            });


            builder.Entity<DocumentEntity>(entity => 
            {
                entity.HasKey(x => x.Id);

                entity
                    .HasMany(x => x.DocumentBlobs)
                    .WithOne(x => x.ParentDocument)
                    .HasForeignKey(x => x.ParentDocumentId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<DocumentBlobEntity>(entity =>
            {
                entity.HasKey(x => x.Id);
            });

            builder.Entity<BlobEntity>(entity =>
            {
                entity.HasKey(x => x.Id);
            });


            builder.Entity<DocumentSubcategoryRelationEntity>(entity =>
            {
                entity.HasKey(x => new { x.DocumentId, x.SubcategoryId });

                entity
                    .HasOne(x => x.Document)
                    .WithMany(x => x.RelatedParentSubcategories)
                    .HasForeignKey(x => x.DocumentId);

                entity
                    .HasOne(x => x.Subcategory)
                    .WithMany(x => x.RelatedChildrenDocuments)
                    .HasForeignKey(x => x.SubcategoryId);
            });


            base.OnModelCreating(builder);
        }


        private void OnSaveChangesInterceptor()
        {
            var currentMomenet = _timeProviderService != null ? 
                _timeProviderService.Now : DateTime.UtcNow;

            var addedEntities = ChangeTracker
                .Entries()
                .Where(p => p.State == EntityState.Added)
                .ToArray();

            foreach (var entry in addedEntities)
            {
                if (entry.Entity is ITrackableEntity trackableEntity)
                {
                    trackableEntity.CreatedOn = currentMomenet;
                    trackableEntity.LastUpdatedOn = currentMomenet;
                }
            }

            var modifiedEntities = ChangeTracker
                .Entries()
                .Where(p => p.State == EntityState.Modified)
                .ToArray();

            foreach (var entry in modifiedEntities)
            {
                if (entry.Entity is ITrackableEntity trackableEntity)
                {
                    trackableEntity.LastUpdatedOn = currentMomenet;
                }
            }

            var deletedEntities = ChangeTracker
                .Entries()
                .Where(p => p.State == EntityState.Deleted)
                .ToArray();

            foreach (var entry in deletedEntities)
            {
                if (entry.Entity is ITrackableEntity trackableEntity)
                {
                    trackableEntity.LastUpdatedOn = currentMomenet;
                }

                if (entry.Entity is ISoftEntity softEntity)
                {
                    entry.State = EntityState.Modified;
                    softEntity.IsDeleted = true;
                    softEntity.DeletedOn = currentMomenet;
                }
            }
        }

        public override int SaveChanges()
        {
            OnSaveChangesInterceptor();
            return base.SaveChanges();
        }


        public override int SaveChanges(
            bool acceptAllChangesOnSuccess)
        {
            OnSaveChangesInterceptor();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }


        public override Task<int> SaveChangesAsync(
            bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default)
        {
            OnSaveChangesInterceptor();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }


        public override Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            OnSaveChangesInterceptor();
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
