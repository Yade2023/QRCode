using System.ComponentModel.DataAnnotations;

namespace QRCodeSystem.Core.Entities
{
    public class Department
    {
        public Department()
        {
            DepartmentName = string.Empty;
            EmployeePositions = new List<EmployeePosition>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string DepartmentName { get; set; }

        // Navigation property
        public virtual ICollection<EmployeePosition> EmployeePositions { get; set; }
    }
}