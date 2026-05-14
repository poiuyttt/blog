using BlogApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Data
{
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

        /// <summary>
        /// 配置实体关系（可选，如果已经用特性配置则不是必须）
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 配置 Post 和 Comment 之间的一对多关系
            modelBuilder.Entity<Post>()
                .HasMany(p => p.Comments) // 一篇文章有多个评论
                .WithOne(c => c.Post)  // 一个评论属于一篇文章
                .HasForeignKey(c => c.PostId)// 外键是 Comment.PostId
                .OnDelete(DeleteBehavior.Cascade); // 删除文章时级联删除评论
        }











    }
}
