<script setup lang="ts">
import { useArticle, useRoute } from "vue-router";
import { useRouter } from "vue-router";

const route = useRoute();
const router = useRouter();

const articleId = route.params.id as string;

function goToHome(): void {
  router.push("/");
}
function goToArticleList(id: number): void {
  router.push({ name: "article", params: { id: id } });
}

console.log("当前文章ID:", articleId);
</script>
<template>
  <div>
    <h2>文章详情</h2>
    <p>文章ID: {{ articleId }}</p>
  </div>
</template>
