using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MatieAvalonia.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<BookingStatus> BookingStatuses { get; set; }

    public virtual DbSet<CardUser> CardUsers { get; set; }

    public virtual DbSet<Collection> Collections { get; set; }

    public virtual DbSet<Master> Masters { get; set; }

    public virtual DbSet<Qualification> Qualifications { get; set; }

    public virtual DbSet<Request> Requests { get; set; }

    public virtual DbSet<RequestStatus> RequestStatuses { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost ;Port= 5555;Database= MatieDB;Username= postgres;Password= 1111;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.IdBooking).HasName("Booking_pkey");

            entity.ToTable("Booking");

            entity.Property(e => e.IdBooking).HasColumnName("ID_Booking");
            entity.Property(e => e.DateTime).HasColumnType("timestamp without time zone");
            entity.Property(e => e.MasterId).HasColumnName("Master_id");
            entity.Property(e => e.ServiceId).HasColumnName("Service_id");
            entity.Property(e => e.StatusId).HasColumnName("Status_id");
            entity.Property(e => e.TypeId).HasColumnName("Type_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("Updated_At");
            entity.Property(e => e.UserId).HasColumnName("User_id");

            entity.HasOne(d => d.Master).WithMany(p => p.BookingMasters)
                .HasForeignKey(d => d.MasterId)
                .HasConstraintName("Master_Booking");

            entity.HasOne(d => d.Service).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("Service_Booking");

            entity.HasOne(d => d.Status).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("Booking_Status");

            entity.HasOne(d => d.User).WithMany(p => p.BookingUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("User_Booking");
        });

        modelBuilder.Entity<BookingStatus>(entity =>
        {
            entity.HasKey(e => e.IdBookingStatus).HasName("BookingStatus_pkey");

            entity.ToTable("BookingStatus");

            entity.Property(e => e.IdBookingStatus).HasColumnName("ID_BookingStatus");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("Name ");
        });

        modelBuilder.Entity<CardUser>(entity =>
        {
            entity.HasKey(e => e.IdCard).HasName("CardUser_pkey");

            entity.ToTable("CardUser");

            entity.Property(e => e.IdCard).HasColumnName("ID_Card");
            entity.Property(e => e.Cvv)
                .HasMaxLength(3)
                .HasColumnName("CVV");
            entity.Property(e => e.DateCard).HasColumnType("timestamp without time zone");
            entity.Property(e => e.NumberCard).HasMaxLength(20);
            entity.Property(e => e.UserId).HasColumnName("User_id");

            entity.HasOne(d => d.User).WithMany(p => p.CardUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("User_Card");
        });

        modelBuilder.Entity<Collection>(entity =>
        {
            entity.HasKey(e => e.IdCollection).HasName("Collection_pkey");

            entity.ToTable("Collection");

            entity.Property(e => e.IdCollection).HasColumnName("ID_Collection");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Master>(entity =>
        {
            entity.HasKey(e => e.IdMaster).HasName("Master_pkey");

            entity.ToTable("Master");

            entity.Property(e => e.IdMaster).HasColumnName("ID_Master");
            entity.Property(e => e.QualifId).HasColumnName("Qualif_id");
            entity.Property(e => e.UserId).HasColumnName("User_id");

            entity.HasOne(d => d.Qualif).WithMany(p => p.Masters)
                .HasForeignKey(d => d.QualifId)
                .HasConstraintName("Master_Qualif");

            entity.HasOne(d => d.User).WithMany(p => p.Masters)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("User_Master");
        });

        modelBuilder.Entity<Qualification>(entity =>
        {
            entity.HasKey(e => e.IdQualif).HasName("Qualification_pkey");

            entity.ToTable("Qualification");

            entity.Property(e => e.IdQualif).HasColumnName("ID_Qualif");
            entity.Property(e => e.Name).HasColumnType("character varying");
        });

        modelBuilder.Entity<Request>(entity =>
        {
            entity.HasKey(e => e.IdRequest).HasName("Request_pkey");

            entity.ToTable("Request");

            entity.Property(e => e.IdRequest).HasColumnName("ID_Request");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("Created_At");
            entity.Property(e => e.QuialifId).HasColumnName("Quialif_id");
            entity.Property(e => e.ReviewedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("Reviewed_At");
            entity.Property(e => e.StatusId).HasColumnName("Status_id");
            entity.Property(e => e.UserId).HasColumnName("User_id");

            entity.HasOne(d => d.Quialif).WithMany(p => p.Requests)
                .HasForeignKey(d => d.QuialifId)
                .HasConstraintName("Qualif_Request");

            entity.HasOne(d => d.Status).WithMany(p => p.Requests)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("Request_Status");

            entity.HasOne(d => d.User).WithMany(p => p.Requests)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("User_Request");
        });

        modelBuilder.Entity<RequestStatus>(entity =>
        {
            entity.HasKey(e => e.IdStatus).HasName("RequestStatuses_pkey");

            entity.Property(e => e.IdStatus).HasColumnName("ID_Status");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.IdReview).HasName("Reviews_pkey");

            entity.Property(e => e.IdReview).HasColumnName("ID_Review");
            entity.Property(e => e.Comment).HasMaxLength(255);
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("Created_At");
            entity.Property(e => e.MasterId).HasColumnName("Master_id");
            entity.Property(e => e.ServiceId).HasColumnName("Service_id");
            entity.Property(e => e.UserId).HasColumnName("User_id");

            entity.HasOne(d => d.Master).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.MasterId)
                .HasConstraintName("Master_Review");

            entity.HasOne(d => d.Service).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("Service_Review");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("User_Rating");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.IdRole).HasName("Role_pkey");

            entity.ToTable("Role");

            entity.Property(e => e.IdRole).HasColumnName("ID_Role");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.IdService).HasName("Services_pkey");

            entity.Property(e => e.IdService).HasColumnName("ID_Service");
            entity.Property(e => e.CollectionId).HasColumnName("Collection_id");
            entity.Property(e => e.Description).HasMaxLength(50);
            entity.Property(e => e.ImgPath).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Price).HasPrecision(7, 2);
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("Updated_at");

            entity.HasOne(d => d.Collection).WithMany(p => p.Services)
                .HasForeignKey(d => d.CollectionId)
                .HasConstraintName("Collection_Services");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUser).HasName("User_pkey");

            entity.ToTable("User");

            entity.Property(e => e.IdUser).HasColumnName("ID_User");
            entity.Property(e => e.Balance).HasPrecision(20, 2);
            entity.Property(e => e.Fname).HasMaxLength(50);
            entity.Property(e => e.Login).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(50);
            entity.Property(e => e.Patronymic).HasMaxLength(50);
            entity.Property(e => e.RoleId).HasColumnName("Role_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("Updated_At");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("User_Role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
