using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QRCodeSystem.Core.Entities
{
    public class EmployeePosition
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int DepartmentId { get; set; }

        [Required]
        [StringLength(50)]
        public string Position { get; set; }

        [Required]
        [StringLength(20)]
        public string EmployeeNumber { get; set; }

        [Required]
        public DateTime HireDate { get; set; }

        [NotMapped]
        public int YearsOfService => (int)((DateTime.Now - HireDate).TotalDays / 365);

        // Navigation properties
        public virtual Department Department { get; set; }
        public virtual EmployeeBasicInfo Employee { get; set; }
    }
}
