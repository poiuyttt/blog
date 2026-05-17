<script setup lang="ts">
import { ref, onMounted } from "vue";
import { useRouter, useRoute } from "vue-router";
import { getCategories, type Category } from "../api/posts";

const categories = ref<Category[]>([]);
const selectedCategoryId = ref<number | null>(null);
const router = useRouter();
const route = useRoute();

const loadCategories = async () => {
  try {
    const res = await getCategories();
    categories.value = res.data;
  } catch (error) {
    console.error("获取分类失败:", error);
  }
};

const handleCategoryClick = (categoryId: number | null) => {
  selectedCategoryId.value = categoryId;
  // 点击分类跳转到首页并带上分类筛选
  router.push({ path: "/", query: { category: categoryId || "" } });
};

// 检查路由参数，如果有 category 参数就选中
onMounted(() => {
  loadCategories();
  if (route.query.category) {
    selectedCategoryId.value = parseInt(route.query.category as string) || null;
  }
});
</script>

<template>
  <div class="sidebar">
    <div class="sidebar-section">
      <h3>分类</h3>
      <div class="category-list">
        <div
          class="category-item"
          :class="{ active: selectedCategoryId === null }"
          @click="handleCategoryClick(null)"
        >
          全部
        </div>
        <div
          v-for="category in categories"
          :key="category.id"
          class="category-item"
          :class="{ active: selectedCategoryId === category.id }"
          @click="handleCategoryClick(category.id)"
        >
          {{ category.name }}
          <span class="count">({{ category.postCount }})</span>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.sidebar {
  width: 250px;
  flex-shrink: 0;
}

.sidebar-section {
  background: white;
  padding: 15px;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
}

.sidebar-section h3 {
  margin: 0 0 10px 0;
  font-size: 16px;
  color: #2c3e50;
  border-left: 3px solid #409eff;
  padding-left: 10px;
}

.category-list {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.category-item {
  padding: 8px 12px;
  border-radius: 4px;
  cursor: pointer;
  transition: all 0.3s;
  color: #606266;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.category-item:hover {
  background: #f5f7fa;
  color: #409eff;
}

.category-item.active {
  background: #409eff;
  color: white;
}

.category-item .count {
  color: #909399;
  font-size: 12px;
}

.category-item.active .count {
  color: rgba(255, 255, 255, 0.8);
}
</style>
