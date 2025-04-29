using System.ComponentModel.DataAnnotations;

namespace QRCodeSystem.Core.Entities
{
    public class QRCode
    {
        public QRCode()
        {
            Id = string.Empty;
            GeneratedBy = string.Empty;
            GeneratedTime = DateTime.Now;
            ExpirationTime = DateTime.Now.AddMinutes(5);
        }

        [Key]
        [StringLength(450)]
        public string Id { get; set; }

        [Required]
        [StringLength(20)]
        public string GeneratedBy { get; set; }

        [Required]
        public DateTime GeneratedTime { get; set; }

        [Required]
        public DateTime ExpirationTime { get; set; }

        // Navigation property
        public virtual EmployeeBasicInfo? Generator { get; set; }
    }
}