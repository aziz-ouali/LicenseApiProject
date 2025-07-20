using LicenseApiProject.Data;
using LicenseApiProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;  // إضافة المكتبة

namespace LicenseApiProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LicensesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LicensesController(AppDbContext context)
        {
            _context = context;
        }

        // نموذج استقبال بيانات الترخيص
        public class LicenseRequest
        {
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public string PhoneNumber { get; set; } = string.Empty;
            public string DeviceIdentifier { get; set; } = string.Empty;
        }

        // نموذج رد مع رسالة نجاح أو خطأ فقط (بدون رخصة)
        public class LicenseResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; } = string.Empty;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterLicense([FromBody] LicenseRequest request)
        {
            // 1. تحقق من وجود المستخدم
            var user = await _context.Users
                .Include(u => u.Licenses)
                .FirstOrDefaultAsync(u => u.Username == request.Username);

            if (user == null)
            {
                // إنشاء مستخدم جديد مع تشفير كلمة المرور
                user = new User
                {
                    Username = request.Username,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password), // تشفير كلمة المرور
                    PhoneNumber = request.PhoneNumber
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }
            else
            {
                // لو المستخدم موجود يمكن التحقق من كلمة المرور (اختياري)
                bool validPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
                if (!validPassword)
                {
                    return BadRequest(new LicenseResponse
                    {
                        Success = false,
                        Message = "كلمة المرور غير صحيحة."
                    });
                }
            }

            // 2. تحقق من وجود الجهاز
            var device = await _context.Devices
                .FirstOrDefaultAsync(d => d.DeviceIdentifier == request.DeviceIdentifier);

            if (device == null)
            {
                device = new Device
                {
                    DeviceIdentifier = request.DeviceIdentifier
                };
                _context.Devices.Add(device);
                await _context.SaveChangesAsync();
            }

            // 3. تحقق وجود رخصة مسبقة للمستخدم والجهاز
            var existingLicense = await _context.Licenses
                .FirstOrDefaultAsync(l => l.UserID == user.UserID && l.DeviceID == device.DeviceID);

            if (existingLicense != null)
            {
                return BadRequest(new LicenseResponse
                {
                    Success = false,
                    Message = "هذا الجهاز مسجل مسبقاً لهذا المستخدم."
                });
            }

            // 4. إنشاء رخصة جديدة (نولد المفتاح فقط للاستخدام الداخلي)
            string licenseKey = GenerateLicenseKey(request.Username, request.DeviceIdentifier, request.Password, request.PhoneNumber);

            var license = new License
            {
                UserID = user.UserID,
                DeviceID = device.DeviceID,
                LicenseKey = licenseKey,
                CreatedAt = DateTime.Now
            };
            _context.Licenses.Add(license);
            await _context.SaveChangesAsync();

            // 5. نرجع فقط رسالة نجاح بدون إرسال الترخيص في الرد
            return Ok(new LicenseResponse
            {
                Success = true,
                Message = "تم إنشاء الترخيص بنجاح. سيتم إرسال الرخصة عبر الهاتف."
            });
        }

        // منطق توليد الرخصة (يمكن تطويره لاحقاً)
        private string GenerateLicenseKey(string username, string device, string password, string phone)
        {
            string part1 = username.Length >= 3 ? username.Substring(0, 3).ToUpper() : username.ToUpper();
            string part2 = device.Length >= 3 ? device.Substring(device.Length - 3).ToUpper() : device.ToUpper();
            string part3 = password.Length >= 2 ? password.Substring(0, 2).ToUpper() : password.ToUpper();
            string part4 = phone.Length >= 4 ? phone.Substring(phone.Length - 4) : phone;

            return $"{part1[0]}{part4[0]}{part2[0]}{part3[0]}-{part1[1]}{part4[1]}{part2[1]}{part3[1]}-{part1[2]}{part4[2]}{part2[2]}{part3[1]}";
        }
    }
}
