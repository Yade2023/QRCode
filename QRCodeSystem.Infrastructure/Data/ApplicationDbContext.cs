using Microsoft.EntityFrameworkCore;
using QRCodeSystem.Core.Entities;

namespace QRCodeSystem.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // 使用單數形式，對應資料庫表名
        public DbSet<EmployeeBasicInfo> EmployeeBasicInfo { get; set; }
        public DbSet<Department> Department { get; set; }
        public DbSet<EmployeePosition> EmployeePosition { get; set; }
        public DbSet<EmployeeLeave> EmployeeLeave { get; set; }
        public DbSet<QRCodeAccessLog> QRCodeAccessLog { get; set; }
        public DbSet<QRCode> QRCodes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 明確指定表名
            modelBuilder.Entity<EmployeeBasicInfo>().ToTable("EmployeeBasicInfo");
            modelBuilder.Entity<Department>().ToTable("Department");
            modelBuilder.Entity<EmployeePosition>().ToTable("EmployeePosition");
            modelBuilder.Entity<EmployeeLeave>().ToTable("EmployeeLeave");
            modelBuilder.Entity<QRCodeAccessLog>().ToTable("QRCodeAccessLog");

            // 配置關聯關係
            modelBuilder.Entity<EmployeeBasicInfo>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.EmployeeNumber).IsUnique();
            });

            modelBuilder.Entity<EmployeePosition>(entity =>
            {
                entity.HasKey(e => e.Id);

                // 設定與部門的關聯
                entity.HasOne(e => e.Department)
                    .WithMany(d => d.EmployeePositions)
                    .HasForeignKey(e => e.DepartmentId)
                    .OnDelete(DeleteBehavior.Restrict);

                // 設定與員工基本資料的關聯
                entity.HasOne(e => e.Employee)
                    .WithOne(e => e.Position)
                    .HasForeignKey<EmployeePosition>(e => e.EmployeeNumber)
                    .HasPrincipalKey<EmployeeBasicInfo>(e => e.EmployeeNumber)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<EmployeeLeave>(entity =>
            {
                entity.HasKey(e => e.Id);

                // 設定與員工基本資料的關聯
                entity.HasOne(e => e.Employee)
                    .WithOne(e => e.Leave)
                    .HasForeignKey<EmployeeLeave>(e => e.EmployeeNumber)
                    .HasPrincipalKey<EmployeeBasicInfo>(e => e.EmployeeNumber)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<QRCodeAccessLog>(entity =>
            {
                entity.HasKey(e => e.Id);

                // 設定與員工基本資料的關聯
                entity.HasOne(e => e.Employee)
                    .WithMany(e => e.AccessLogs)
                    .HasForeignKey(e => e.EmployeeNumber)
                    .HasPrincipalKey(e => e.EmployeeNumber)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}