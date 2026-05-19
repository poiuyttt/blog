<script setup lang="ts">
import BlogHeader from "./components/BlogHeader.vue";
import BlogFooter from "./components/BlogFooter.vue";
import BlogCard from "./components/BlogCard.vue";
import NavBar from "./components/NavBar.vue";
</script>

<template>
  <BlogHeader />
  <NavBar />
  <RouterView />
  <main class="main-content">
    <BlogCard
      title="开始学习全栈"
      summary="从C#到Vue3，记录我的全栈学习之路"
      date="2026-04-24"
    />
    <BlogCard
      title="掌握Props传参"
      summary="今天学会了父组件向子组件传递参数"
    />
  </main>

  <BlogFooter />
</template>

<style>
body {
  margin: 0;
  font-family: Arial, sans-serif;
}

.main-content {
  max-width: 800px;
  margin: 0 auto;
  padding: 20px;
}
</style>
