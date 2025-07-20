using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LicenseApiProject.Data;
using LicenseApiProject.Models;

namespace LicenseApiProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActivationController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ActivationController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("validate")]
        public async Task<IActionResult> ValidateLicense([FromBody] ValidateLicenseRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.DeviceId) || string.IsNullOrWhiteSpace(request.ActivationCode))
            {
                return BadRequest(new LicenseResponse
                {
                    Success = false,
                    Message = "جميع الحقول مطلوبة."
                });
            }

            var device = await _context.Devices
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.DeviceIdentifier == request.DeviceId);

            if (device == null)
            {
                return NotFound(new LicenseResponse
                {
                    Success = false,
                    Message = "الجهاز غير مسجل."
                });
            }

            var license = await _context.Licenses
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.DeviceID == device.DeviceID && l.LicenseKey == request.ActivationCode);

            if (license == null)
            {
                return Unauthorized(new LicenseResponse
                {
                    Success = false,
                    Message = "رمز التفعيل غير صحيح."
                });
            }

            return Ok(new LicenseResponse
            {
                Success = true,
                Message = "تم التحقق من الترخيص بنجاح."
            });
        }
    }

    public class ValidateLicenseRequest
    {
        public string DeviceId { get; set; } = string.Empty;
        public string ActivationCode { get; set; } = string.Empty;
    }

    public class LicenseResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
