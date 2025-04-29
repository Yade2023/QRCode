using System;
using System.ComponentModel.DataAnnotations;

namespace QRCodeSystem.Core.Entities
{
    public class EmployeeLeave
    {
        [Required] // 添加 required 修饰符
        public string EmployeeNumber { get; set; }

        public EmployeeLeave(string employeeNumber)
        {
            EmployeeNumber = employeeNumber ?? throw new ArgumentNullException(nameof(employeeNumber));
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]

        public int CompensatoryLeave { get; set; }
        public int SpecialLeave { get; set; }
        public int PersonalLeave { get; set; }
        public int OfficialLeave { get; set; }
        public int SickLeave { get; set; }
        public int BereavementLeave { get; set; }
        public int MarriageLeave { get; set; }
        public int BusinessTrip { get; set; }
        public int OtherLeave { get; set; }

        // Navigation property
        public virtual EmployeeBasicInfo Employee { get; set; }
    }
}
