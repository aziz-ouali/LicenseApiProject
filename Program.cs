using LicenseApiProject.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// إضافة خدمات الـ Controllers
builder.Services.AddControllers();

// الحصول على سلسلة الاتصال من متغير البيئة "DATABASE_URL" أو من appsettings.json
string connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") 
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

// تهيئة EF Core مع MySQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// إضافة أدوات Swagger لتوثيق الـ API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// تمكين Swagger في جميع البيئات
app.UseSwagger();
app.UseSwaggerUI();

// توجيه الطلبات لـ HTTPS
app.UseHttpsRedirection();

// تفعيل التفويض (Authorization)
app.UseAuthorization();

// ربط الـ Controllers بالمسارات
app.MapControllers();

// تحديد المنفذ من متغير البيئة PORT (Railway) أو الافتراضي 8080
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Add($"http://*:{port}");

// تشغيل التطبيق
app.Run();
