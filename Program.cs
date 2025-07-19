using LicenseApiProject.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// إضافة خدمات الـ Controllers
builder.Services.AddControllers();

// ربط قاعدة البيانات
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// إضافة أدوات Swagger لتوثيق الـ API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// تمكين Swagger في جميع البيئات (ليست فقط Development)
app.UseSwagger();
app.UseSwaggerUI();

// توجيه الطلبات لـ HTTPS
app.UseHttpsRedirection();

// تفعيل التفويض (Authorization)
app.UseAuthorization();

// ربط الـ Controllers بالمسارات
app.MapControllers();

// ❗ هذا السطر مهم لتحديد منفذ التشغيل بشكل ديناميكي في Railway
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Add($"http://*:{port}");

// تشغيل التطبيق
app.Run();
