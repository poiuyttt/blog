# 📝 博客系统（BlogApi + BlogWeb）

基于 ASP.NET Core 8 + Vue 3 的全博客应用，支持文章的 Markdown 编辑与预览、分类管理、评论系统、JWT 用户认证、文件上传等功能。

## 技术栈

### 后端 — BlogApi

| 类别 | 技术 |
|------|------|
| 运行时 | .NET 8.0 |
| ORM | Entity Framework Core 9.0 |
| 数据库 | SQL Server |
| 认证 | JWT Bearer（自定义策略授权） |
| API 文档 | Swagger（Swashbuckle） |

### 前端 — BlogWeb

| 类别 | 技术 |
|------|------|
| 框架 | Vue 3.5 + TypeScript 6 |
| 构建 | Vite 8 |
| 状态管理 | Pinia 3（持久化插件） |
| 路由 | Vue Router 4 |
| UI 库 | Element Plus 2.13 |
| HTTP | Axios 1.15 |
| Markdown | @kangc/v-md-editor（GitHub 主题 + 代码高亮 + 行号） |

## 项目结构

```
├── BlogApi/BlogApi/           # 后端 API
│   ├── Controllers/           # 5 个 API 控制器
│   │   ├── AuthController.cs        # 注册、登录、改密、上传头像
│   │   ├── PostController.cs        # 文章 CRUD、分页、搜索、分类筛选
│   │   ├── CommentController.cs     # 评论创建、删除
│   │   ├── CategoryController.cs    # 分类管理
│   │   └── FileController.cs        # 文件上传/下载/删除
│   ├── Services/              # 业务逻辑层（5 个 Service）
│   ├── Models/                # 实体 + DTO
│   ├── Data/                  # AppDbContext + EF Core 配置
│   ├── Authorization/         # 自定义授权策略（AdminOrAuthor 等）
│   ├── Filters/               # 全局异常过滤器 + 请求日志过滤器
│   ├── Middlewares/           # 请求计时中间件
│   ├── Program.cs             # 入口、DI 注册、中间件管线
│   └── Migrations/            # 9 次增量式数据库迁移
│
├── BlogWeb/                   # 前端
│   └── src/
│       ├── api/               # Axios API 封装（posts/auth/comments/user）
│       ├── views/             # 9 个页面
│       │   ├── HomePage.vue         # 首页（分页文章列表 + 分类筛选）
│       │   ├── ArticlePage.vue      # 文章详情（Markdown + 目录 + 上下篇）
│       │   ├── ArticleEdit.vue      # 写文章/编辑（Markdown 编辑器）
│       │   ├── LoginPage.vue        # 登录
│       │   ├── RegisterPage.vue     # 注册
│       │   ├── ProfilePage.vue      # 个人中心（头像、资料、改密）
│       │   ├── SearchPage.vue       # 搜索
│       │   ├── ImageGallery.vue     # 图库
│       │   └── AboutPage.vue        # 关于
│       ├── components/        # 公共组件
│       │   ├── BlogHeader.vue       # 导航栏 + 搜索框
│       │   ├── BlogFooter.vue       # 页脚
│       │   ├── PostList.vue         # 文章列表（分页组件）
│       │   ├── CommentList.vue      # 评论列表 + 发表
│       │   ├── CategorySidebar.vue  # 分类侧边栏
│       │   ├── BlogCard.vue         # 文章卡片
│       │   ├── FileUpload.vue       # 文件上传
│       │   └── ConfirmButton.vue    # 确认按钮
│       ├── stores/            # Pinia 状态管理
│       │   ├── auth.ts              # 认证状态（token + 用户信息）
│       │   └── search.ts            # 搜索状态（持久化）
│       ├── router/index.ts    # 路由（含导航守卫）
│       ├── utils/             # 工具函数
│       │   ├── request.ts           # Axios 实例（拦截器）
│       │   ├── auth.ts              # JWT 解析 + 角色判断
│       │   └── format.ts            # 日期格式化
│       └── types/             # TypeScript 类型声明
│
└── TodoAPI/                   # 另一个练习用 Todo API
```

## 功能特性

### 已完成

- ✅ 用户注册 / 登录（JWT 认证）
- ✅ 文章 Markdown 编辑与预览（v-md-editor）
- ✅ 文章分页列表、分类筛选
- ✅ 文章搜索（标题/摘要/内容全文检索）
- ✅ 评论系统（发表/删除）
- ✅ 分类管理（Admin）
- ✅ 文件上传（图片）与图库
- ✅ 个人中心（头像上传、资料编辑、密码修改）
- ✅ 文章目录导航（TOC）
- ✅ 上一篇 / 下一篇导航
- ✅ 角色权限（Admin / User）
- ✅ 请求日志 + 全局异常处理
- ✅ 请求计时中间件
- ✅ CORS 跨域配置
- ✅ Swagger API 文档

### 开发中 / 待完善

- ⬜ 密码哈希替换为 BCrypt（当前为 Base64 编码）
- ⬜ 刷新令牌（Refresh Token）机制
- ⬜ 评论分页
- ⬜ 单元测试
- ⬜ Rate limiting

## 快速开始

### 环境要求

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js](https://nodejs.org/) 18+
- [SQL Server](https://www.microsoft.com/sql-server)（LocalDB / Express / Developer 均可）
- 可选：Visual Studio / Rider / VS Code

### 1. 克隆并配置数据库

```bash
# 还原 NuGet 包
cd BlogApi/BlogApi
dotnet restore
```

确保 `appsettings.json` 中的连接串指向你的 SQL Server 实例：

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=BlogDb;Trusted_Connection=true;TrustServerCertificate=true;"
}
```

### 2. 运行数据库迁移

```bash
dotnet ef database update
```

此命令会执行 `Migrations/` 目录下的所有迁移，自动创建数据库和表。

> 如果 `dotnet ef` 命令不可用，先安装：`dotnet tool install --global dotnet-ef`

### 3. 启动后端

```bash
# 端口以 launchSettings.json 为准
dotnet run
```

启动后访问：
- API 基址：`http://localhost:5256` 或 `https://localhost:7126`
- Swagger 文档：`https://localhost:7126/swagger`

### 4. 启动前端

```bash
cd BlogWeb
npm install
npm run dev
```

启动后访问：`http://localhost:5173`

### 默认端口

| 服务 | URL |
|------|-----|
| 后端 HTTP | `http://localhost:5256` |
| 后端 HTTPS | `https://localhost:7126` |
| 前端开发服务器 | `http://localhost:5173` |
| Swagger | `https://localhost:7126/swagger` |

> 如遇 HTTPS 证书问题，可改用 HTTP 端口。前端 `request.ts` 中的 `baseURL` 需要对应修改。

## API 概览

### 认证

| 方法 | 路径 | 说明 | 认证 |
|------|------|------|------|
| POST | `/api/auth/register` | 注册 | ❌ |
| POST | `/api/auth/login` | 登录 | ❌ |
| PUT | `/api/auth/profile` | 更新个人信息 | ✅ |
| PUT | `/api/auth/change-password` | 修改密码 | ✅ |
| GET | `/api/auth/check-username` | 检查用户名可用性 | ❌ |
| POST | `/api/auth/upload-avatar` | 上传头像 | ✅ |

### 文章

| 方法 | 路径 | 说明 | 认证 |
|------|------|------|------|
| GET | `/api/post` | 获取全部文章 | ❌ |
| GET | `/api/post/{id}` | 获取文章详情 | ❌ |
| GET | `/api/post/paged` | 分页文章 | ❌ |
| GET | `/api/post/search` | 搜索文章 | ❌ |
| GET | `/api/post/category/{id}` | 按分类获取 | ❌ |
| POST | `/api/post` | 创建文章 | ✅ |
| PUT | `/api/post/{id}` | 更新文章 | ✅（作者/Admin） |
| DELETE | `/api/post/{id}` | 删除文章 | ✅（作者/Admin） |

### 评论

| 方法 | 路径 | 说明 | 认证 |
|------|------|------|------|
| GET | `/api/comment?postId={id}` | 获取文章评论 | ❌ |
| POST | `/api/comment` | 发表评论 | ✅ |
| DELETE | `/api/comment/{id}` | 删除评论 | ✅（作者/Admin） |

### 分类

| 方法 | 路径 | 说明 | 认证 |
|------|------|------|------|
| GET | `/api/category` | 获取全部分类 | ❌ |
| POST | `/api/category` | 创建分类 | ✅（Admin） |
| DELETE | `/api/category/{id}` | 删除分类 | ✅（Admin） |

### 文件

| 方法 | 路径 | 说明 | 认证 |
|------|------|------|------|
| POST | `/api/file/upload` | 上传文件 | ✅（Admin） |
| GET | `/api/file/download` | 下载文件 | ❌ |
| DELETE | `/api/file/delete` | 删除文件 | ✅（Admin） |
| GET | `/api/file/list` | 文件列表 | ❌ |

## 配置说明

后端配置位于 `BlogApi/BlogApi/appsettings.json`：

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=BlogDb;..."
  },
  "Jwt": {
    "Key": "你的密钥（至少 32 字符）",
    "Issuer": "BlogApi",
    "Audience": "BlogApi",
    "ExpireMinutes": 60
  }
}
```

前端 API 基址在 `BlogWeb/src/utils/request.ts` 中设置：

```typescript
baseURL: "https://localhost:7126/api"   // 改为你后端的实际地址
```

## 数据库设计

### 表结构

```
Users      1──N Posts       (UserId → Id)
Users      1──N Comments    (UserId → Id)
Categories 1──N Posts       (CategoryId → Id)
Posts      1──N Comments    (PostId → Id)
```

### 实体关系

| 关系 | 删除行为 |
|------|---------|
| Post → Comments | Cascade（删文章连带删评论） |
| User → Posts | SetNull（删用户后文章作者置空） |
| User → Comments | SetNull（删用户后评论作者置空） |
| Category → Posts | SetNull（删分类后文章分类置空） |

## 许可证

本项目为个人练习项目。
