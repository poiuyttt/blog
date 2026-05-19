<script setup lang="ts">
import { useCounterStore } from "../stores/counter";

const counterStore = useCounterStore();
</script>
<template>
  <div>
    <h2>关于</h2>
    <p>这是一个用 Vue3 + TypeScript 构建的博客系统。</p>
  </div>
  <div>
    <h2>关于页面</h2>
    <p>{{ counterStore.count }}</p>
    <button @click="counterStore.increment">+1</button>
  </div>
</template>
