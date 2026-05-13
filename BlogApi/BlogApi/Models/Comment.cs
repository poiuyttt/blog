using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogApi.Models
{
    [Table("Comments")]
    public class Comment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        [StringLength(100)]
        public string Author { get; set; } = "当前用户"; // 暂时固定，后续接入认证

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// 外键：指向 Post 表的 Id
        /// </summary>
        public int PostId { get; set; } // 外键，关联到 Post 实体

        /// <summary>
        /// 导航属性：所属的文章
        /// </summary>
        [ForeignKey("PostId")]
        public virtual Post Post { get; set; } = null!; // 导航属性，指向所属的文章
    }
}
