<script setup lang="ts">
import { ref } from "vue";

const siteTitle = ref<string>("我的全栈博客");
const siteDescription = ref<string>("分享学习心得与开发经验");
</script>

<template>
  <header class="blog-header">
    <div class="container">
      <h1>{{ siteTitle }}</h1>
      <p>{{ siteDescription }}</p>
    </div>
  </header>
</template>

<style scoped>
.blog-header {
  background-color: #2c3e50;
  color: white;
  padding: 30px 0;
  text-align: center;
  margin-button: 30px;
}
.blog-header h1 {
  margin: 0;
  font-size: 28px;
}
.blog-header p {
  margin: 10px 0 0;
  font-size: 16px;
  opacity: 0.8;
}
</style>
