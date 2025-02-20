using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using test.Models.Entity;
using Microsoft.Extensions.Configuration;

namespace test.Services;

public partial class ApplicationDbContext : DbContext
{
    private readonly string _connectionString;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, string connectionString = "DefaultConnection")
        : base(options)
    {
        _connectionString = connectionString;
    }

    public virtual DbSet<Mail> Mail { get; set; }
    public virtual DbSet<Notification> Notification { get; set; }
    public virtual DbSet<User> User { get; set; }
    public virtual DbSet<Role> Role { get; set; }
    public virtual DbSet<UserRole> UserRole { get; set; }
    public virtual DbSet<Module> Module { get; set; }
    public virtual DbSet<RoleModule> RoleModule { get; set; }
    public virtual DbSet<UserModule> UserModule { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            
            var connectionString = configuration.GetConnectionString(_connectionString);
            optionsBuilder.UseSqlServer(connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Mail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_MailService");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateBy).HasMaxLength(100);
            entity.Property(e => e.CreateTime).HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.EmailBcc)
                .HasMaxLength(2000)
                .HasColumnName("EmailBCC");
            entity.Property(e => e.EmailCc)
                .HasMaxLength(2000)
                .HasColumnName("EmailCC");
            entity.Property(e => e.EmailContent).HasColumnType("ntext");
            entity.Property(e => e.FileAttach).HasMaxLength(1000);
            entity.Property(e => e.FromDate).HasColumnType("datetime");
            entity.Property(e => e.Location).HasMaxLength(500);
            entity.Property(e => e.Organizer).HasMaxLength(500);
            entity.Property(e => e.OrganizerMail)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.SendTime).HasColumnType("datetime");
            entity.Property(e => e.SentStatus).HasMaxLength(1000);
            entity.Property(e => e.Subject).HasColumnType("ntext");
            entity.Property(e => e.ToDate).HasColumnType("datetime");
            entity.Property(e => e.Uid)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("UID");
        });
        //User configuration
        modelBuilder.Entity<User>()
            .HasMany(u => u.UserRoles)
            .WithOne(ur => ur.User)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        //Role configuration
        modelBuilder.Entity<Role>()
            .HasMany(r => r.UserRoles)
            .WithOne(ur => ur.Role)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
        //Module configuration
        modelBuilder.Entity<Module>()
            .HasMany(m => m.UserModules)
            .WithOne(um => um.Module)
            .HasForeignKey(um => um.ModuleId)
            .OnDelete(DeleteBehavior.Cascade);
        // UserRole configuration
        modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId);

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId);

        // RoleModule configuration
        modelBuilder.Entity<RoleModule>()
            .HasKey(rm => new { rm.RoleId, rm.ModuleId });

        modelBuilder.Entity<RoleModule>()
            .HasOne(rm => rm.Role)
            .WithMany(r => r.RoleModules)
            .HasForeignKey(rm => rm.RoleId);

        modelBuilder.Entity<RoleModule>()
            .HasOne(rm => rm.Module)
            .WithMany(m => m.RoleModules)
            .HasForeignKey(rm => rm.ModuleId);

        // UserModule configuration
        modelBuilder.Entity<UserModule>()
            .HasKey(um => new { um.UserId, um.ModuleId });

        modelBuilder.Entity<UserModule>()
            .HasOne(um => um.User)
            .WithMany(u => u.UserModules)
            .HasForeignKey(um => um.UserId);

        modelBuilder.Entity<UserModule>()
            .HasOne(um => um.Module)
            .WithMany(m => m.UserModules)
            .HasForeignKey(um => um.ModuleId);
    
        }

    // partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
