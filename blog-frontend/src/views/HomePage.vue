<script setup lang="ts">
import { useCounterStore } from "../stores/counter";
import { ref, onMounted } from "vue";
import { getPosts, type Post } from "../api/posts";
import { useRouter } from "vue-router";

const counterStore = useCounterStore();

const posts = ref<Post[]>([]);

const router = useRouter();

const searchKeyword = ref<string>("");

function goToSearch(): void {
  if (searchKeyword.value.trim() === "") {
    alert("请输入搜索关键字");
    return;
  }
  router.push({
    path: "/search",
    query: { keyword: searchKeyword.value, page: 1 },
  });
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
  </div>
  <!--   <div class="counter-demo">
    <h2>Pinia计数器</h2>
    <p>当前计数：{{ counterStore.count }}</p>
    <p>双倍值：{{ counterStore.doubleCount }}</p>
    <button @click="counterStore.increment">+1</button>
    <button @click="counterStore.reset">重置</button>
    <button @click="counterStore.decrement">-1</button>
  </div> -->
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
</style>
