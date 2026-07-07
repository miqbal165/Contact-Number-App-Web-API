using ContactNumberWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ContactNumberWebAPI.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<ContactCategory> ContactCategories => Set<ContactCategory>();
    
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditInformation();
        return base.SaveChangesAsync(cancellationToken);
    }
    
    public override int SaveChanges()
    {
        ApplyAuditInformation();
        return base.SaveChanges();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(user => user.Id);
            entity.Property(user => user.Id).ValueGeneratedNever();
            entity.HasIndex(user => user.Email).IsUnique();
            entity.Property(user => user.FullName).HasMaxLength(100).IsRequired();
            entity.Property(user => user.Email).HasMaxLength(150).IsRequired();
            entity.Property(user => user.PasswordHash).IsRequired();
            entity.Property(user => user.Role).HasMaxLength(50).IsRequired();
            entity.Property(user => user.CreatedAt).IsRequired();
        });

        modelBuilder.Entity<Contact>(entity =>
        {
            entity.HasKey(contact => contact.Id);
            entity.Property(contact => contact.Id).ValueGeneratedNever();
            entity.Property(contact => contact.Name).HasMaxLength(100).IsRequired();
            entity.Property(contact => contact.PhoneNumber).HasMaxLength(15).IsRequired();
            entity.Property(contact => contact.Email).HasMaxLength(150);
            entity.Property(contact => contact.Address).HasMaxLength(250);
            entity.Property(contact => contact.CreatedAt).IsRequired();

            entity.HasOne(contact => contact.ContactCategory)
                .WithMany(category => category.Contacts)
                .HasForeignKey(contact => contact.ContactCategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ContactCategory>(entity =>
        {
            entity.HasKey(contactCategory => contactCategory.Id);
            entity.Property(contactCategory => contactCategory.Id).ValueGeneratedNever();
            entity.HasIndex(category => category.Name).IsUnique();
            entity.Property(category => category.Name).HasMaxLength(100).IsRequired();
            entity.Property(category => category.CreatedAt).IsRequired();
        });
    }


    private void ApplyAuditInformation()
    {
        DateTime now = DateTime.Now;
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is not User &&
                entry.Entity is not Contact &&
                entry.Entity is not ContactCategory)
            {
                continue;
            }

            if (entry.State == EntityState.Added)
            {
                entry.Property(nameof(User.CreatedAt)).CurrentValue = now;
                entry.Property(nameof(User.UpdatedAt)).CurrentValue = null;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Property(nameof(User.CreatedAt)).IsModified = false;
                entry.Property(nameof(User.UpdatedAt)).CurrentValue = now;
            }
        }
    }
    
    
}