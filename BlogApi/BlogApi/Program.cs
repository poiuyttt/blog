using BlogApi.Authorization;
using BlogApi.Data;
using BlogApi.Filters;
using BlogApi.Middlewares;
using BlogApi.Models;
using BlogApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.LogTo(Console.WriteLine, LogLevel.Information);
    }
});

//AddCors:注册CORS服务
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowVueClient",
        policy =>
        {
            policy
                .WithOrigins("http://localhost:5173", "http://localhost:5174")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        }
    );
});

// 绑定 JWT 配置
var jwtSettings = new JwtSettings();
builder.Configuration.GetSection("Jwt").Bind(jwtSettings);

// 注册 JWT 认证服务
builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
        };
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOrAuthor", policy =>
        policy.Requirements.Add(new IsAuthorRequirement()));
    options.AddPolicy("AdminOrCommentAuthor", policy =>
        policy.Requirements.Add(new CommentAuthorRequirement()));
});

// Add services to the container.
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddSingleton<IFileService, FileService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddSingleton(jwtSettings);
builder.Services.AddScoped<IAuthorizationHandler, IsAuthorHandler>();
builder.Services.AddScoped<IAuthorizationHandler, CommentAuthorHandler>();
builder.Services.AddSingleton<GlobalExceptionFilter>();
builder.Services.AddSingleton<ActionLoggingFilter>();
builder
    .Services.AddControllers(options =>
    {
        options.Filters.Add<GlobalExceptionFilter>();
        options.Filters.Add<ActionLoggingFilter>();
    })
    .AddJsonOptions(options =>
    {
        // 解决前后端字段名大小写不匹配的问题
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        // 解决循环引用问题
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 添加静态文件服务（必须在 UseRouting 之前）
// UseStaticFiles：将 wwwroot 文件夹暴露为静态文件目录
// 同时将 uploads 文件夹也设置为可静态访问
app.UseStaticFiles();

app.UseStaticFiles(
    new StaticFileOptions
    {
        // 将物理路径 ./uploads 映射到 URL 路径 /uploads
        FileProvider = new PhysicalFileProvider(
            Path.Combine(Directory.GetCurrentDirectory(), "uploads")
        ),
        RequestPath = "/uploads",
    }
);

// 注意：UseCors 必须在 UseRouting 之前调用
app.UseCors("AllowVueClient");

//app.UseMiddleware<RequestTimingMiddleware>();
app.UseRequestTiming();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
