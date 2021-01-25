using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using AutionApp;
using Microsoft.AspNetCore.Identity;

namespace AutionApp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.EnableSensitiveDataLogging(true);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bid>((Action<Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Bid>>)(entity =>
            {
                entity.Property(e => e.Rate)
                    .HasColumnType("money");
            }));

            modelBuilder.Entity<Category>((Action<Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Category>>)(entity =>
            {
                //entity.HasOne((System.Linq.Expressions.Expression<Func<Category, Category>>)(d => (Category)d.Parent))
                //    .WithMany(p => p.Parents)
                //    .HasForeignKey((System.Linq.Expressions.Expression<Func<Category, object>>)(d => (object)d.Parent))
                //    .HasConstraintName("FK_Categories_Categories");
            }));

            modelBuilder.Entity<Lot>(entity =>
            {
                entity.Property(e => e.Step)
                    .HasColumnType("money");

                entity.Property(e => e.StartPrice)
                    .HasColumnType("money");
            });

            modelBuilder.Entity<StatesLots>(entity =>
            {
                entity.HasKey(e => new { e.LotId, e.StateId, e.Time });
            });

            modelBuilder.Entity<Sell>(entity =>
            {
                entity.HasKey(e => new { e.LotId, e.UserId });
            });

            base.OnModelCreating(modelBuilder);
        }

        public virtual DbSet<Bid> Bids { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Feedback> Feedbacks { get; set; }
        public virtual DbSet<Lot> Lots { get; set; }
        public virtual DbSet<Sell> Sells { get; set; }
        public virtual DbSet<User> Users { get; set; }
        //public virtual DbSet<Roles> Roles { get; set; }
        public virtual DbSet<State> States { get; set; }
        public virtual DbSet<StatesLots> StatesLots { get; set; }
        public DbSet<AutionApp.UserRole> UserRole { get; set; }
        //public virtual DbSet<User> Users { get; set; }
    }
}
