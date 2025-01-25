
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using test.Models.Entity;

namespace test.Services;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Mail> Mail { get; set; }
    public virtual DbSet<User> User { get; set; }
    public virtual DbSet<Role> Role { get; set; }
    public virtual DbSet<UserRole> UserRole { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Name=ConnectionStrings:DefaultConnection");
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
   
   
    }

    // partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
