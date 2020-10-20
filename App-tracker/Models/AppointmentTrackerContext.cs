using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace App_tracker.Models
{
    public partial class AppointmentTrackerContext : DbContext
    {
        public AppointmentTrackerContext()
        {
        }

        public AppointmentTrackerContext(DbContextOptions<AppointmentTrackerContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Bays> Bays { get; set; }
        public virtual DbSet<ContainerComments> ContainerComments { get; set; }
        public virtual DbSet<ContainerDepartments> ContainerDepartments { get; set; }
        public virtual DbSet<ContainerSuppliers> ContainerSuppliers { get; set; }
        public virtual DbSet<ContainerTypes> ContainerTypes { get; set; }
        public virtual DbSet<Containers> Containers { get; set; }
        public virtual DbSet<Suppliers> Suppliers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Data Source=DESKTOP-9FIIT8Q\\SQLEXPRESS;Database=AppointmentTracker;Trusted_Connection=True;MultipleActiveResultSets=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bays>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Bay).HasColumnName("bay");
            });

            modelBuilder.Entity<ContainerComments>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Comment)
                    .IsRequired()
                    .HasColumnName("comment");

                entity.Property(e => e.ContainerId).HasColumnName("containerId");

                entity.HasOne(d => d.Container)
                    .WithMany(p => p.ContainerComments)
                    .HasForeignKey(d => d.ContainerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ContainerComments_ContainerId");
            });

            modelBuilder.Entity<ContainerDepartments>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Department)
                    .IsRequired()
                    .HasColumnName("department")
                    .HasMaxLength(55)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ContainerSuppliers>(entity =>
            {
                entity.HasKey(e => new { e.ContainerId, e.SupplierId });

                entity.Property(e => e.ContainerId).HasColumnName("containerId");

                entity.Property(e => e.SupplierId).HasColumnName("supplierId");

                entity.HasOne(d => d.Container)
                    .WithMany(p => p.ContainerSuppliers)
                    .HasForeignKey(d => d.ContainerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ContainerSuppliers_Containers");

                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.ContainerSuppliers)
                    .HasForeignKey(d => d.SupplierId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ContainerSuppliers_Suppliers");
            });

            modelBuilder.Entity<ContainerTypes>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnName("type")
                    .HasMaxLength(55)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Containers>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ActNumOfPallets).HasColumnName("actNumOfPallets");

                entity.Property(e => e.ActNumOfUnits).HasColumnName("actNumOfUnits");

                entity.Property(e => e.ActTimeOfArrival).HasColumnName("actTimeOfArrival");

                entity.Property(e => e.ArrivalDate)
                    .HasColumnName("arrivalDate")
                    .HasColumnType("date");

                entity.Property(e => e.Bay).HasColumnName("bay");

                entity.Property(e => e.BayId).HasColumnName("bayId");

                entity.Property(e => e.CheckSheet).HasColumnName("checkSheet");

                entity.Property(e => e.DepartmentId).HasColumnName("departmentId");

                entity.Property(e => e.Door)
                    .HasColumnName("door")
                    .HasMaxLength(50);

                entity.Property(e => e.ExpNumOfPallets).HasColumnName("expNumOfPallets");

                entity.Property(e => e.ExpNumOfUnits).HasColumnName("expNumOfUnits");

                entity.Property(e => e.ExpTimeOfArrival).HasColumnName("expTimeOfArrival");

                entity.Property(e => e.ItemDiscrepancy).HasColumnName("itemDiscrepancy");

                entity.Property(e => e.RefNum)
                    .IsRequired()
                    .HasColumnName("refNum")
                    .IsUnicode(false);

                entity.Property(e => e.TypeId).HasColumnName("typeId");

                entity.HasOne(d => d.BayNavigation)
                    .WithMany(p => p.Containers)
                    .HasForeignKey(d => d.BayId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("containers_bayId_FK");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Containers)
                    .HasForeignKey(d => d.DepartmentId)
                    .HasConstraintName("FK_Containers_ContainerDepartments");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.Containers)
                    .HasForeignKey(d => d.TypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Containers_ContainerTypes");
            });

            modelBuilder.Entity<Suppliers>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Supplier)
                    .IsRequired()
                    .HasColumnName("supplier")
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
