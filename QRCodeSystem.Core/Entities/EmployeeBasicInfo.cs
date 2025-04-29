using System.ComponentModel.DataAnnotations;

namespace QRCodeSystem.Core.Entities
{
    public class EmployeeBasicInfo
    {
        public EmployeeBasicInfo()
        {
            EmployeeNumber = string.Empty;
            Name = string.Empty;
            Phone = string.Empty;
            Address = string.Empty;
            Email = string.Empty;
            AccessLogs = new List<QRCodeAccessLog>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string EmployeeNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string? Email { get; set; }

        // Navigation properties
        public virtual EmployeePosition? Position { get; set; }
        public virtual EmployeeLeave? Leave { get; set; }
        public virtual ICollection<QRCodeAccessLog> AccessLogs { get; set; }
    }
}