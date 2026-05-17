<script setup lang="ts">
import BlogHeader from "./components/BlogHeader.vue";
import BlogFooter from "./components/BlogFooter.vue";
</script>

<template>
  <BlogHeader />
  <main>
    <RouterView />
  </main>
  <BlogFooter />
</template>

<style>
body {
  margin: 0;
  font-family: Arial, sans-serif;
  background-color: #f5f5f5;
}
main {
  min-height: calc(100vh - 200px);
  padding: 20px 0;
}
</style>
