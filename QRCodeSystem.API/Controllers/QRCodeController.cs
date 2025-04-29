using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRCodeSystem.Core.Entities;
using QRCodeSystem.Infrastructure.Data;
using System.ComponentModel.DataAnnotations;

namespace QRCodeSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QRCodeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public QRCodeController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateQRCode([FromBody] GenerateQRCodeRequest request)
        {
            try
            {
                // 驗證管理員工號
                var employee = await _context.EmployeeBasicInfo
                    .Include(e => e.Position)
                    .FirstOrDefaultAsync(e => e.EmployeeNumber == request.EmployeeId);

                if (employee == null)
                {
                    return NotFound(new { 
                        success = false,
                        message = "找不到此員工" 
                    });
                }

                // 檢查是否為管理員
                var allowedPositions = new[] { "總經理", "製造主管", "人事專員" }; // 根據您的資料庫現有職位
                if (!allowedPositions.Contains(employee.Position?.Position ?? ""))
                {
                    return BadRequest(new { 
                        success = false,
                        message = "您沒有權限生成 QR Code" 
                    });
                }

                // 生成唯一的 QRCode ID
                string qrCodeId = Guid.NewGuid().ToString();
                
                // 生成時間戳（包含過期時間）
                DateTime expirationTime = DateTime.Now.AddMinutes(5);
                string timestamp = expirationTime.ToString("yyyyMMddHHmmss");

                // 儲存 QR Code 記錄
                var qrCode = new QRCode
                {
                    Id = qrCodeId,
                    GeneratedBy = request.EmployeeId,
                    GeneratedTime = DateTime.Now,
                    ExpirationTime = expirationTime
                };

                _context.QRCodes.Add(qrCode);
                await _context.SaveChangesAsync();

                // 產生 QR Code 的內容（包含檢查頁面的 URL）
                var qrCodeData = $"http://localhost:5173/checkin?id={qrCodeId}&t={timestamp}";

                return Ok(new { 
                    success = true,
                    message = "QR Code 生成成功",
                    qrCodeData = qrCodeData
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false,
                    message = $"QR Code 生成失敗: {ex.Message}" 
                });
            }
        }

        [HttpPost("checkin")]
        public async Task<IActionResult> Checkin([FromBody] CheckinRequest request)
        {
            try
            {
                // 驗證 QR Code
                var qrCode = await _context.QRCodes
                    .FirstOrDefaultAsync(q => q.Id == request.QRCodeId);

                if (qrCode == null)
                {
                    return NotFound(new { 
                        success = false,
                        message = "無效的 QR Code" 
                    });
                }

                // 檢查是否過期
                if (DateTime.Now > qrCode.ExpirationTime)
                {
                    return BadRequest(new { 
                        success = false,
                        message = "QR Code 已過期" 
                    });
                }

                // 驗證員工
                var employee = await _context.EmployeeBasicInfo
                    .FirstOrDefaultAsync(e => e.EmployeeNumber == request.EmployeeNumber);

                if (employee == null)
                {
                    return NotFound(new { 
                        success = false,
                        message = "找不到此員工" 
                    });
                }

                // 記錄打卡
                var accessLog = new QRCodeAccessLog
                {
                    EmployeeNumber = request.EmployeeNumber,
                    QRCodeId = request.QRCodeId,
                    AccessTime = DateTime.Now
                };

                _context.QRCodeAccessLog.Add(accessLog);
                await _context.SaveChangesAsync();

                return Ok(new { 
                    success = true,
                    message = "打卡成功",
                    data = new {
                        employeeName = employee.Name,
                        checkinTime = accessLog.AccessTime
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false,
                    message = $"打卡失敗: {ex.Message}" 
                });
            }
        }
    }

    public class GenerateQRCodeRequest
    {
        [Required]
        public string EmployeeId { get; set; } = string.Empty;
    }

    public class CheckinRequest
    {
        [Required]
        public string EmployeeNumber { get; set; } = string.Empty;

        [Required]
        public string QRCodeId { get; set; } = string.Empty;

        [Required]
        public string Timestamp { get; set; } = string.Empty;
    }
}