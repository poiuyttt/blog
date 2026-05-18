using BlogApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Data;

/// <summary>
/// 数据库上下文类——应用程序与数据库交互的桥梁
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// 通过构造函数注入数据库连接配置
    /// </summary>
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    /// <summary>
    /// DbSet：代表数据库中的表，可通过 LINQ 查询
    /// </summary>
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();

    /// <summary>
    /// 配置实体关系
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Post ↔ Comment（一对多）
        modelBuilder.Entity<Post>()
            .HasMany(p => p.Comments)
            .WithOne(c => c.Post)
            .HasForeignKey(c => c.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        // User → Post（一对多，文章作者）
        modelBuilder.Entity<User>()
            .HasMany(u => u.Posts)
            .WithOne(p => p.User)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        // User → Comment（一对多，评论作者）
        modelBuilder.Entity<User>()
            .HasMany(u => u.Comments)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        // Category → Post（一对多）
        modelBuilder.Entity<Category>()
            .HasMany(c => c.Posts)
            .WithOne(p => p.Category)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
