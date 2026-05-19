<script setup lang="ts">
import { useArticle, useRoute } from "vue-router";
import { useRouter } from "vue-router";

const route = useRoute();
const router = useRouter();

function goToHome(): void {
  router.push("/");
}
function goToArticleList(id: number): void {
  router.push({ name: "article", params: { id: id } });
}

console.log("文章ID:", route.params.id);
</script>
<template>
  <div>
    <h2>文章详情</h2>
    <p>文章ID: {{ route.params.id }}</p>
  </div>
</template>
