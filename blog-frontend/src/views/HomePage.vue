<script setup lang="ts">
import { ref, reactive, onMounted } from "vue";
import { useCounterStore } from "../stores/counter";
import { getPosts, type Post } from "../api/posts";
import { useRouter } from "vue-router";
import { useSearchStore } from "../stores/search";
import { ElMessage, ElTable, ElTableColumn } from "element-plus";

const counterStore = useCounterStore();

const posts = ref<Post[]>([]);

const router = useRouter();
const searchStore = useSearchStore();

const searchKeyword = ref<string>("");
const form = reactive({
  title: "",
});

// 表格数据，用于存储待办事项列表
const tableData = ref([
  { id: 1, title: "学习ASP.NET Core", isCompleta: false },
  { id: 2, title: "学习Vue3", isCompleta: true },
]);

const handleSubmit = () => {
  if (!form.title.trim()) {
    ElMessage.error("请输入待办事项");
    return;
  }
  const newItem = {
    id: Date.now(),
    title: form.title,
    isCompleta: false,
  };
  tableData.value.push(newItem);
  form.title = "";
  ElMessage.success("添加成功");
};

function goToSearch(): void {
  if (searchKeyword.value.trim() === "") {
    alert("请输入搜索关键字");
    return;
  }
  searchStore.setKeyword(searchKeyword.value);
  searchStore.setPage(1);
  router.push("/search");
}

onMounted(async () => {
  try {
    posts.value = await getPosts();
  } catch (error) {
    console.log("获取文章失败，使用模拟数据展示效果");
    posts.value = [
      {
        id: 1,
        title: "开始学习全栈",
        content: "从 C# 到 Vue3，记录我的全栈学习之路。",
        author: "张三",
        createdAt: "2026-04-20",
      },
      {
        id: 2,
        title: "异步编程与 Axios",
        content: "今天学会了用 Axios 发送网络请求，并配置了拦截器。",
        author: "张三",
        createdAt: "2026-04-27",
      },
    ];
  }
});
</script>
<template>
  <div>
    <h2>首页</h2>
    <div class="search-box">
      <input
        v-model="searchKeyword"
        placeholder="搜索文章..."
        @keyup.enter="goToSearch"
      />
      <button @click="goToSearch">搜索</button>
    </div>
    <p>欢迎来到我的全栈博客！</p>
  </div>
  <div class="home">
    <h2>文章列表</h2>
    <div v-if="posts.length === 0">加载中...</div>
    <div v-for="post in posts" :key="post.id" class="post-card">
      <h3>{{ post.title }}</h3>
      <p class="meta">{{ post.author }}·{{ post.createdAt }}</p>
      <p>{{ post.content }}</p>
    </div>
    <h2>待办事项管理</h2>
    <el-form :model="form" class="todo-form">
      <el-form-item label="新事项">
        <el-input v-model="form.title" placeholder="请输入新事项"></el-input>
      </el-form-item>
      <el-form-item>
        <el-button type="primary" @click="handleSubmit">添加</el-button>
      </el-form-item>
    </el-form>

    <el-table :data="tableData" style="width: 100%">
      <el-table-column prop="id" label="ID" width="80" />
      <el-table-column prop="title" label="标题" />
      <el-table-column label="状态" width="100">
        <template #default="scope">
          <el-tag
            :type="scope.row.isCompleta ? 'success' : 'info'"
            disable-transitions
          >
            {{ scope.row.isCompleta ? "已完成" : "未完成" }}
          </el-tag>
        </template>
      </el-table-column>
    </el-table>
  </div>
  <div class="counter-demo">
    <h2>Pinia计数器</h2>
    <p>当前计数：{{ counterStore.count }}</p>
    <p>双倍值：{{ counterStore.doubleCount }}</p>
    <button @click="counterStore.increment">+1</button>
    <button @click="counterStore.reset">重置</button>
    <button @click="counterStore.decrement">-1</button>
  </div>
</template>
<style scoped>
.counter-demo {
  text-align: center;
  padding: 30px;
}

.buttons {
  display: flex;
  justify-content: center;
  gap: 10px;
  margin-top: 15px;
}

button {
  padding: 8px 20px;
  font-size: 16px;
  cursor: pointer;
}

.post-card {
  background: white;
  border: 1px solid #ddd;
  border-radius: 8px;
  padding: 20px;
  margin-bottom: 15px;
}

.meta {
  color: #888;
  font-size: 14px;
}

.search-box {
  margin-bottom: 20px;
}

.search-box input {
  padding: 8px;
  width: 250px;
  margin-right: 10px;
}

.home {
  max-width: 600px;
  margin: 0 auto;
  padding: 20px;
}
.todo-form {
  margin-bottom: 30px;
  background: white;
  padding: 20px;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}
</style>
