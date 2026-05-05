<script setup lang="ts">
import { useCounterStore } from "../stores/counter";

const counterStore = useCounterStore();
</script>
<template>
  <div>
    <h2>关于页面</h2>
    <p>这是一个用 Vue3 + TypeScript 构建的博客系统。</p>
  </div>
</template>
