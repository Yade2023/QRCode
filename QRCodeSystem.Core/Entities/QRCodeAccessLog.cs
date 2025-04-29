using System;
using System.ComponentModel.DataAnnotations;

namespace QRCodeSystem.Core.Entities
{
    public class QRCodeAccessLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string EmployeeNumber { get; set; }

        [Required]
        public DateTime AccessTime { get; set; } = DateTime.Now;

        [Required]
        [StringLength(100)]
        public string QRCodeId { get; set; }

        // Navigation property
        public virtual EmployeeBasicInfo Employee { get; set; }
    }
}
