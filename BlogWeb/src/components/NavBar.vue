<script setup lang="ts"></script>
<template>
  <nav>
    <RouterLink to="/">首页</RouterLink>
    <RouterLink to="/article/:id">文章</RouterLink>
    <!--     <RouterLink to="/search">搜索</RouterLink>
 -->
    <RouterLink to="/about">关于</RouterLink>
  </nav>
</template>
<style scoped>
nav {
  display: flex;
  gap: 20px;
  padding: 15px;
  background-color: #f5f5f5;
  margin-bottom: 20px;
}
</style>
