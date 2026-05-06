<script setup lang="ts">
interface Props {
  title: string;
  summary: string;
  date?: string;
}

const props = defineProps<Props>();
</script>
<template>
  <div class="card">
    <h2>{{ props.title }}</h2>
    <p>{{ props.summary }}</p>
    <span v-if="props.date" class="date">{{ props.date }}</span>
  </div>
</template>
<style scoped>
.card {
  background: white;
  border: 1px solid #ddd;
  border-radius: 8px;
  padding: 20px;
  margin-bottom: 15px;
}
.date {
  color: #999;
  font-size: 14px;
}
</style>
