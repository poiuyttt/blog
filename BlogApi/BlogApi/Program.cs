using BlogApi.Data;
using BlogApi.Filters;
using BlogApi.Middlewares;
using BlogApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});


//AddCors:注册CORS服务
builder.Services.AddCors(Options =>
{ // AddPolicy：定义一个名为 "AllowVueClient" 的 CORS 策略
    Options.AddPolicy(
        "AllowVueClient",
        policy =>
        {
            // WithOrigins：允许来自这些地址的跨域请求
            // 开发环境：Vite 默认端口 5173/5174
            policy
                .WithOrigins("http://localhost:5173", "http://localhost:5174")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        }
    );
});

// Add services to the container.

builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddSingleton<IFileService, FileService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddSingleton<GlobalExceptionFilter>();
builder.Services.AddSingleton<ActionLoggingFilter>();
builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
    options.Filters.Add<ActionLoggingFilter>();
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.Configure<PaginationSettings>(builder.Configuration.GetSection("Pagination"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 📌 添加静态文件服务（必须在 UseRouting 之前）
// UseStaticFiles：将 wwwroot 文件夹暴露为静态文件目录
// 同时将 uploads 文件夹也设置为可静态访问
app.UseStaticFiles();  // 默认使用 wwwroot 目录

// 如果需要直接访问 uploads 目录中的文件（如 /uploads/20260509/abc.jpg）
// 使用 UseStaticFiles 的另一个重载来映射物理路径到 URL 路径
app.UseStaticFiles(new StaticFileOptions
{
    // 将物理路径 ./uploads 映射到 URL 路径 /uploads
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "uploads")),
    RequestPath = "/uploads"
});

// 注意：UseCors 必须在 UseRouting 之前调用
app.UseCors("AllowVueClient");

//app.UseMiddleware<RequestTimingMiddlewares>();
app.UseRequestTiming();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();