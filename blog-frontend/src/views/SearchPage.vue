<script setup lang="ts">
import { useRoute } from "vue-router";
import { useSearchStore } from "../stores/search";

const route = useRoute();
const searchStore = useSearchStore();
/* const keyword = route.query.keyword as string;
const page = route.query.page as string; */
</script>
<template>
  <div>
    <h2>搜索结果</h2>
    <p>
      你在搜索:<strong> {{ searchStore.keyword }}</strong>
    </p>
    <div class="update-keyword">
      <label>修改搜索词：</label>
      <input
        :value="searchStore.keyword"
        @input="
          searchStore.setKeyword(($event.target as HTMLInputElement).value)
        "
        placeholder="输入新关键词"
      />
    </div>
    <p>当前页码: {{ searchStore.page }}</p>
    <p v-if="searchStore.keyword" class="persist-hint"></p>
    <p v-else class="persost-hint"></p>
    <button @click="searchStore.reset()" class="reset-btn">重置搜索</button>
  </div>
</template>
<style scoped>
.update-keyword {
  margin: 15px 0;
}
.update-keyword input {
  padding: 6px;
  width: 200px;
  margin-left: 10px;
  border: 1px solid #ddd;
  border-radius: 4px;
}
.persist-hint {
  color: #888;
  font-size: 14px;
  margin-top: 15px;
}
.reset-btn {
  padding: 8px 16px;
  background-color: #e74c3c;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  margin-top: 15px;
}
.reset-btn:hover {
  background-color: #c0392b;
}
</style>
