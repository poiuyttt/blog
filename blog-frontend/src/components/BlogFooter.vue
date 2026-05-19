<script setup lang="ts">
import { computed } from "vue";

const currentYear = computed<number>(() => new Date().getFullYear());
</script>

<template>
  <footer class="blog-footer">
    <div class="container">
      <p>&copy; {{ currentYear }} 我的全栈博客 · 保留所有权利</p>
    </div>
  </footer>
</template>

<style scoped>
.blog-footer {
  background-color: #2c3e50;
  color: white;
  padding: 20px 0;
  text-align: center;
  margin-top: 50px;
}

.blog-footer p {
  margin: 0;
  font-size: 14px;
  opacity: 0.8;
}
</style>
