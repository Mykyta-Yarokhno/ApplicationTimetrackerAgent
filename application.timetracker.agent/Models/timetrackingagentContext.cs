using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace application.timetracker.agent.Models
{
    public partial class timetrackingagentContext : DbContext
    {
        public timetrackingagentContext()
        {
        }

        public timetrackingagentContext(DbContextOptions<timetrackingagentContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AppRunningTime> AppRunningTimes { get; set; }
        public virtual DbSet<Application> Applications { get; set; }
        public virtual DbSet<Ose> Oses { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }
    

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Cyrillic_General_CS_AS");

            
            modelBuilder.Entity<AppRunningTime>(entity =>
            {
                entity.HasKey(e => new { e.StartTime, e.AppId, e.UserId });

                entity.ToTable("app_running_time");

                
                entity.Property(e => e.StartTime)
                    .HasColumnType("datetime")
                    .HasColumnName("start_time");

                entity.Property(e => e.AppId).HasColumnName("app_id");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.FinishTime)
                    .HasColumnType("datetime")
                    .HasColumnName("finish_time");

                entity.HasOne(d => d.App)
                    .WithMany(p => p.AppRunningTimes)
                    .HasForeignKey(d => d.AppId)
                    .HasConstraintName("FK_app_running_time_1_0");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AppRunningTimes)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_app_running_time_0_0");
            });

            modelBuilder.Entity<Application>(entity =>
            {
                entity.ToTable("applications");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Ose>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("oses");

                entity.Property(e => e.Os)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("OS")
                    .IsFixedLength(true);

                entity.Property(e => e.Platform)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("platform")
                    .IsFixedLength(true);

                entity.Property(e => e.UserId).HasColumnName("user_id");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("name");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
