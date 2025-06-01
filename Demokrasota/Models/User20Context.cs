using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Demokrasota.Models;

public partial class User20Context : DbContext
{
    public User20Context()
    {
    }

    public User20Context(DbContextOptions<User20Context> options)
        : base(options)
    {
    }

    public virtual DbSet<EmployeeService> EmployeeServices { get; set; }

    public virtual DbSet<IndividualClient> IndividualClients { get; set; }

    public virtual DbSet<LegalClient> LegalClients { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Position> Positions { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=45.67.56.214;Port=5421;Database=user20;Username=user20;Password=uFTJmxd5");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EmployeeService>(entity =>
        {
            entity.HasKey(e => new { e.EmployeeId, e.ServiceId }).HasName("employee_services_pkey");

            entity.ToTable("employee_services", "public2");

            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.ServiceId).HasColumnName("service_id");

            entity.HasOne(d => d.Service).WithMany(p => p.EmployeeServices)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("employee_services_service_id_fkey");
        });

        modelBuilder.Entity<IndividualClient>(entity =>
        {
            entity.HasKey(e => e.ClientCode).HasName("individual_clients_pkey");

            entity.ToTable("individual_clients", "public2");

            entity.Property(e => e.ClientCode)
                .HasMaxLength(20)
                .HasColumnName("client_code");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.BirthDate).HasColumnName("birth_date");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .HasColumnName("full_name");
            entity.Property(e => e.PassportData)
                .HasMaxLength(50)
                .HasColumnName("passport_data");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .HasColumnName("password");
        });

        modelBuilder.Entity<LegalClient>(entity =>
        {
            entity.HasKey(e => e.ClientCode).HasName("legal_clients_pkey");

            entity.ToTable("legal_clients", "public2");

            entity.Property(e => e.ClientCode)
                .HasMaxLength(20)
                .HasColumnName("client_code");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.BankAccount)
                .HasMaxLength(20)
                .HasColumnName("bank_account");
            entity.Property(e => e.Bik)
                .HasMaxLength(20)
                .HasColumnName("bik");
            entity.Property(e => e.CompanyName)
                .HasMaxLength(100)
                .HasColumnName("company_name");
            entity.Property(e => e.ContactPerson)
                .HasMaxLength(100)
                .HasColumnName("contact_person");
            entity.Property(e => e.ContactPhone)
                .HasMaxLength(50)
                .HasColumnName("contact_phone");
            entity.Property(e => e.DirectorName)
                .HasMaxLength(100)
                .HasColumnName("director_name");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Inn)
                .HasMaxLength(20)
                .HasColumnName("inn");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .HasColumnName("password");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("orders_pkey");

            entity.ToTable("orders", "public2");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ClientCode)
                .HasMaxLength(20)
                .HasColumnName("client_code");
            entity.Property(e => e.ClosingDate).HasColumnName("closing_date");
            entity.Property(e => e.CreationDate).HasColumnName("creation_date");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.ExecutionTime)
                .HasMaxLength(20)
                .HasColumnName("execution_time");
            entity.Property(e => e.OrderNumber)
                .HasMaxLength(100)
                .HasColumnName("order_number");
            entity.Property(e => e.Services).HasColumnName("services");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
        });

        modelBuilder.Entity<Position>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("positions_pkey");

            entity.ToTable("positions", "public2");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Role)
                .HasMaxLength(100)
                .HasColumnName("role");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("services_pkey");

            entity.ToTable("services", "public2");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AvgDeviation)
                .HasPrecision(10, 3)
                .HasColumnName("avg_deviation");
            entity.Property(e => e.ExecutionTime)
                .HasMaxLength(20)
                .HasColumnName("execution_time");
            entity.Property(e => e.MeasurementUnit)
                .HasMaxLength(20)
                .HasColumnName("measurement_unit");
            entity.Property(e => e.Price)
                .HasPrecision(10, 2)
                .HasColumnName("price");
            entity.Property(e => e.PriceForRussianCosmetics)
                .HasPrecision(10, 2)
                .HasColumnName("price_for_russian_cosmetics");
            entity.Property(e => e.ServiceCode)
                .HasMaxLength(20)
                .HasColumnName("service_code");
            entity.Property(e => e.ServiceName)
                .HasMaxLength(100)
                .HasColumnName("service_name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
