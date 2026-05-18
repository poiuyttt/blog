<script setup lang="ts">
import { computed, ref, onMounted, onUnmounted } from "vue";
import { useRoute } from "vue-router";
import BlogHeader from "./components/BlogHeader.vue";
import BlogFooter from "./components/BlogFooter.vue";
import CategorySidebar from "./components/CategorySidebar.vue";

const route = useRoute();
const showBackTop = ref(false);

const showSidebar = computed(() => {
  const noSidebarPaths = [
    "/login",
    "/register",
    "/profile",
    "/article/new",
    "/article/:id/edit",
  ];
  const currentPath = route.path;
  // 检查是否匹配不需要侧边栏的路径
  return !noSidebarPaths.some((path) => {
    if (path.includes(":id")) {
      // 处理带参数的路径
      const regex = new RegExp("^" + path.replace(":id", "\\d+") + "$");
      return regex.test(currentPath);
    }
    return currentPath === path;
  });
});

function handleScroll(): void {
  showBackTop.value = window.scrollY > 300;
}

function scrollToTop(): void {
  window.scrollTo({ top: 0, behavior: "smooth" });
}

onMounted(() => {
  window.addEventListener("scroll", handleScroll);
});

onUnmounted(() => {
  window.removeEventListener("scroll", handleScroll);
});
</script>

<template>
  <BlogHeader />
  <main>
    <div v-if="showSidebar" class="app-container">
      <CategorySidebar />
      <div class="main-content">
        <RouterView />
      </div>
    </div>
    <RouterView v-else />
  </main>
  <BlogFooter />

  <div v-if="showBackTop" class="back-to-top">
    <el-button type="primary" @click="scrollToTop">↑ 返回顶部</el-button>
  </div>
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

.app-container {
  display: flex;
  gap: 20px;
  padding: 0 40px;
}

.main-content {
  flex: 1;
}

.back-to-top {
  position: fixed;
  bottom: 40px;
  right: 40px;
  z-index: 1000;
}
</style>
