using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRCodeSystem.Infrastructure.Data;
using QRCodeSystem.Core.Entities;
using System.Threading.Tasks;

namespace QRCodeSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TestController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("test-connection")]
        public IActionResult TestConnection()
        {
            try
            {
                // 測試資料庫連接
                bool canConnect = _context.Database.CanConnect();
                return Ok(new { 
                    Status = "Success", 
                    CanConnect = canConnect,
                    Message = "資料庫連接測試完成。" 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    Status = "Error", 
                    Message = $"資料庫連接測試失敗: {ex.Message}" 
                });
            }
        }

        [HttpGet("departments")]
        public async Task<IActionResult> GetDepartments()
        {
            try
            {
                var departments = await _context.Department.ToListAsync();
                return Ok(new {
                    Status = "Success",
                    Data = departments,
                    Count = departments.Count,
                    Message = "部門資料讀取成功。"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new {
                    Status = "Error",
                    Message = $"讀取部門資料失敗: {ex.Message}"
                });
            }
        }

        [HttpGet("employees")]
        public async Task<IActionResult> GetEmployees()
        {
            try
            {
                var employees = await _context.EmployeeBasicInfo
                    .Include(e => e.Position)
                    .ThenInclude(p => p.Department)
                    .Include(e => e.Leave)
                    .ToListAsync();

                return Ok(new {
                    Status = "Success",
                    Data = employees,
                    Count = employees.Count,
                    Message = "員工資料讀取成功。"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new {
                    Status = "Error",
                    Message = $"讀取員工資料失敗: {ex.Message}"
                });
            }
        }
    }
}