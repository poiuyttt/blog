using BlogApi.Filters;
using BlogApi.Middlewares;
using BlogApi.Services;

var builder = WebApplication.CreateBuilder(args);

//AddCors:注册CORS服务
builder.Services.AddCors(Options =>
{ // AddPolicy：定义一个名为 "AllowVueClient" 的 CORS 策略
    Options.AddPolicy(
        "AllowVueClient",
        policy =>
        {
            // WithOrigins：允许来自这些地址的跨域请求
            // 开发环境：Vite 默认端口 5173
            policy
                .WithOrigins("http://localhost:5173")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        }
    );
});

// Add services to the container.

builder.Services.AddSingleton<IPostService, PostService>();
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