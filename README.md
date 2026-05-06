# 全栈学习项目

基于《完整6周全栈学习计划》的学习代码仓库，涵盖从 C# 基础语法到 ASP.NET Core Web API，再到 Vue3 前端的全栈学习路径。

## 项目结构

```
d:\code\
├── Csharp/                     # C# 基础 → 进阶（13 个递进式控制台项目）
│   ├── ConsoleApp1/            # 数据类型、变量、字符串格式化、作用域、类型转换
│   ├── ConsoleApp2/            # ref/out 参数、params、循环、类与属性
│   ├── ConsoleApp3/            # 接口、抽象类、继承、多态
│   ├── ConsoleApp4/            # 委托、事件、IEnumerable 接口
│   ├── ConsoleApp5/            # 继承与多态深入、属性验证、virtual/override
│   ├── ConsoleApp6/            # 泛型、异常处理（try/catch/finally）
│   ├── ConsoleApp7/            # LINQ（Where、Select、OrderBy、查询表达式）
│   ├── ConsoleApp8/            # 委托、事件、Lambda 表达式、Action<T>
│   ├── ConsoleApp9/            # 实战：发布/订阅日志系统（事件驱动）
│   ├── ConsoleApp10/           # async/await、Task、匿名类型、JSON 序列化
│   ├── ConsoleApp11/           # IConfiguration、appsettings.json、Options 模式
│   ├── ConsoleApp12/           # 依赖注入（DI）、ServiceCollection、接口编程
│   └── ConsoleApp13/           # DI + Logging + Configuration + xUnit 单元测试
│
├── BlogApi/                    # ASP.NET Core 8.0 Web API 实战项目（练习版）
│   └── BlogApi/
│       ├── Controllers/        # API 控制器
│       │   └── TodoController.cs      # Todo CRUD（GET/POST/PUT/DELETE）
│       ├── Models/             # 数据模型
│       │   ├── TodoItem.cs             # 待办事项实体
│       │   ├── Dtos/                   # 数据传输对象
│       │   │   ├── CreateTodoDto.cs    # 创建请求 DTO
│       │   │   └── TodoDto.cs          # 响应 DTO
│       │   └── Configurations/         # 配置模型
│       │       └── PaginationSettings.cs
│       ├── Services/           # 业务逻辑层
│       │   ├── ITodoService.cs         # 服务接口
│       │   └── TodoService.cs          # 服务实现（内存数据存储）
│       ├── Program.cs          # 应用入口与 DI 配置
│       ├── appsettings.json    # 应用配置
│       └── BlogApi.http        # HTTP 请求测试文件
│
├── TodoAPI/                    # ASP.NET Core 8.0 Web API 实战项目（正式版）
│   └── TodoAPI/
│       ├── Controllers/        # API 控制器
│       │   └── TodoController.cs      # Todo CRUD + 结构化日志
│       ├── Models/             # 数据模型
│       │   ├── TodoItem.cs             # 待办事项实体（含 CreatedAt 时间戳）
│       │   └── Dto/                    # 数据传输对象
│       │       ├── CreateTodoDto.cs    # 创建请求 DTO
│       │       └── TodoDto.cs          # 响应 DTO（含 CreatedAt）
│       ├── Services/           # 业务逻辑层
│       │   ├── ITodoService.cs         # 服务接口
│       │   └── TodoService.cs          # 服务实现（内存数据存储 + ILogger）
│       ├── Program.cs          # 应用入口与 DI 配置
│       ├── appsettings.json    # 应用配置
│       └── TodoAPI.http        # HTTP 请求测试文件
│
└── BlogWeb/                    # Vue3 前端项目
    └── src/
        ├── api/                # API 请求封装
        │   └── posts.ts               # 文章接口（axios）
        ├── assets/             # 静态资源
        ├── components/         # 公共组件
        │   ├── BlogHeader.vue          # 博客头部
        │   ├── BlogFooter.vue          # 博客底部（computed 动态年份）
        │   ├── BlogCard.vue            # 文章卡片（Props 传参）
        │   ├── NavBar.vue              # 导航栏（RouterLink）
        │   ├── PostList.vue            # 文章列表（分页 + el-pagination）
        │   └── ConfirmButton.vue       # 确认按钮（emit 事件）
        ├── router/             # 路由配置
        │   └── index.ts               # 路由表 + 导航守卫（token 鉴权）
        ├── stores/             # Pinia 状态管理
        │   ├── counter.ts              # 计数器 Store（computed + actions）
        │   └── search.ts               # 搜索 Store（持久化插件）
        ├── utils/              # 工具函数
        │   └── request.ts              # axios 实例（拦截器 + token 注入）
        ├── views/              # 页面组件
        │   ├── HomePage.vue            # 首页（文章列表 + 搜索跳转）
        │   ├── ArticlePage.vue         # 文章详情（Markdown 渲染）
        │   ├── SearchPage.vue          # 搜索页（Pinia 状态读取）
        │   ├── LoginPage.vue           # 登录页（Element Plus 表单验证）
        │   ├── RegisterPage.vue        # 注册页（自定义验证规则）
        │   └── AboutPage.vue           # 关于页
        ├── App.vue              # 根组件
        ├── main.ts              # 应用入口（Pinia + Router + ElementPlus + VMdEditor）
        └── style.css            # 全局样式
```

## 技术栈

### 后端

| 类别     | 技术                                     |
| -------- | ---------------------------------------- |
| 运行时   | .NET 8.0                                 |
| Web 框架 | ASP.NET Core 8.0                         |
| API 文档 | Swagger / OpenAPI（Swashbuckle 6.6.2）   |
| 序列化   | Newtonsoft.Json                          |
| 日志     | Microsoft.Extensions.Logging             |
| 配置     | Microsoft.Extensions.Configuration       |
| 依赖注入 | Microsoft.Extensions.DependencyInjection |
| 测试     | xUnit                                    |
| IDE      | Rider                                    |

### 前端

| 类别     | 技术                                    |
| -------- | --------------------------------------- |
| 框架     | Vue 3.5                                 |
| 语言     | TypeScript 6.0                          |
| 构建工具 | Vite 8.0                                |
| 路由     | Vue Router 4.6                          |
| 状态管理 | Pinia 3.0 + pinia-plugin-persistedstate |
| UI 库    | Element Plus 2.13                       |
| HTTP     | Axios 1.15                              |
| Markdown | @kangc/v-md-editor 2.3（GitHub 主题）   |
| IDE      | VS Code                                 |

## 学习路径

### 第一阶段：C# 基础（ConsoleApp1 ~ ConsoleApp6）

从零开始掌握 C# 核心语法：

- **ConsoleApp1** — 数据类型、变量声明、`var` 类型推断、字符串格式化（拼接/占位符/插值）、作用域、类型转换
- **ConsoleApp2** — `ref` / `out` / `params` 参数修饰符、`for` / `foreach` / `while` 循环、类与属性、值类型 vs 引用类型
- **ConsoleApp3** — `interface` 接口、`abstract` 抽象类、继承、多态（里氏替换原则）
- **ConsoleApp4** — `delegate` 委托、`event` 事件、`IEnumerable` 与集合遍历
- **ConsoleApp5** — 深入继承与多态、属性 `get`/`set` 访问器与验证逻辑、`virtual`/`override`
- **ConsoleApp6** — 泛型类 `Box<T>`、`try`/`catch`/`finally` 异常处理

### 第二阶段：C# 进阶（ConsoleApp7 ~ ConsoleApp10）

掌握现代 C# 开发的核心能力：

- **ConsoleApp7** — LINQ 查询（方法语法 + 查询表达式）、`Where`/`Select`/`OrderBy`、匿名类型、延迟执行
- **ConsoleApp8** — 委托与事件深入、`Action<T>` 泛型委托、Lambda 表达式、多播委托
- **ConsoleApp9** — **实战**：发布/订阅日志系统，事件驱动架构，控制台 + 文件双订阅者
- **ConsoleApp10** — `async`/`await` 异步编程、`Task`、文件异步读写、匿名类型、Newtonsoft.Json 序列化

### 第三阶段：企业级开发（ConsoleApp11 ~ ConsoleApp13）

引入 .NET 生态的核心基础设施：

- **ConsoleApp11** — `IConfiguration` 配置系统、`appsettings.json`、`Bind()` 绑定、Options 模式
- **ConsoleApp12** — 依赖注入（DI）容器、`ServiceCollection`、`AddSingleton`/`AddTransient`、接口编程
- **ConsoleApp13** — DI + Logging + Configuration 三合一整合、`ILogger<T>` 结构化日志、xUnit 单元测试

### 第四阶段：Web API 实战（BlogApi / TodoAPI）

将前面所有知识整合为完整的后端项目：

- **分层架构**：Controller → Service → Model
- **RESTful API 设计**：`GET` / `POST` / `PUT` / `DELETE` 完整 CRUD
- **DTO 模式**：分离入参（`CreateTodoDto`）与出参（`TodoDto`），对外契约隔离
- **依赖注入**：`ITodoService` 接口注入 Controller
- **Options 模式**：`PaginationSettings` 强类型配置（BlogApi）
- **结构化日志**：`ILogger<T>` 记录请求与异常
- **Swagger 文档**：自动生成 API 文档与在线调试界面

> **BlogApi** 与 **TodoAPI** 的区别：BlogApi 是练习版（含 Options 配置演示），TodoAPI 是正式版（Service 层注入 ILogger、实体含 CreatedAt 时间戳、更完善的日志记录）。

### 第五阶段：Vue3 前端实战（BlogWeb）

从后端走向全栈，构建博客前端应用：

- **项目搭建**：Vite + Vue3 + TypeScript 脚手架
- **路由系统**：Vue Router 路由表配置、懒加载、导航守卫（token 鉴权）
- **状态管理**：Pinia Store（counter / search）、`pinia-plugin-persistedstate` 持久化
- **HTTP 通信**：Axios 实例封装、请求/响应拦截器、token 自动注入
- **UI 组件库**：Element Plus 集成、表单验证（内置规则 + 自定义验证器）
- **Markdown 渲染**：v-md-editor + GitHub 主题 + 代码高亮 + 行号
- **组件通信**：Props 传参（BlogCard）、emit 事件（ConfirmButton）
- **计算属性**：computed 派生状态（分页计算、动态年份）

## 快速开始

### 环境要求

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js](https://nodejs.org/)（建议 18+）
- 后端 IDE：Rider
- 前端 IDE：VS Code

### 运行控制台项目

```bash
# 以 ConsoleApp13 为例
cd Csharp/ConsoleApp13/ConsoleApp13
dotnet run
```

### 运行 BlogApi

```bash
cd BlogApi/BlogApi
dotnet run
```

启动后访问：

- API 基址：`https://localhost:5256`（端口以 `launchSettings.json` 为准）
- Swagger UI：`https://localhost:5256/swagger`

### 运行 TodoAPI

```bash
cd TodoAPI/TodoAPI
dotnet run
```

### 运行 BlogWeb

```bash
cd BlogWeb
npm install
npm run dev
```

### 运行测试

```bash
cd Csharp/ConsoleApp13/ConsoleApp13
dotnet test
```

## API 接口一览

### BlogApi

| 方法     | 路径             | 说明             |
| -------- | ---------------- | ---------------- |
| `GET`    | `/api/todo`      | 获取所有待办事项 |
| `GET`    | `/api/todo/{id}` | 获取单个待办事项 |
| `POST`   | `/api/todo`      | 创建待办事项     |
| `PUT`    | `/api/todo/{id}` | 更新待办事项     |
| `DELETE` | `/api/todo/{id}` | 删除待办事项     |

### TodoAPI

| 方法     | 路径             | 说明             |
| -------- | ---------------- | ---------------- |
| `GET`    | `/api/todo`      | 获取所有待办事项 |
| `GET`    | `/api/todo/{id}` | 获取单个待办事项 |
| `POST`   | `/api/todo`      | 创建待办事项     |
| `PUT`    | `/api/todo/{id}` | 更新待办事项     |
| `DELETE` | `/api/todo/{id}` | 删除待办事项     |

## 代码规范

本项目遵循以下编码约定：

- **C#**：私有字段使用 `_` 前缀驼峰命名（`_logger`），公共属性使用帕斯卡命名，显式定义构造函数，使用 `var` 关键字
- **TypeScript / Vue**：使用 `<script setup lang="ts">` + Composition API，`const`/`let` 禁止 `var`，CSS 类名使用 kebab-case
- **命名空间**：使用文件范围命名空间声明（`namespace Xxx;`）
- **项目结构**：Controller → Service → Model 分层，DTO 独立目录
