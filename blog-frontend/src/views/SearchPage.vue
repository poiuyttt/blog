<script setup lang="ts">
import { useRoute } from "vue-router";

const route = useRoute();

const keyword = route.query.keyword as string;
const page = route.query.page as string;
</script>
<template>
  <div>
    <h2>搜索结果</h2>
    <p>
      你在搜索:<strong> {{ keyword }}</strong>
    </p>
    <p>当前页码: {{ page }}</p>
  </div>
</template>
