<script setup lang="ts">
const emit = defineEmits<{
  (event: "confirm"): void;
  (event: "cancel"): void;
}>();
function handleConfirm(): void {
  emit("confirm");
}
function handleCancle(): void {
  emit("cancel");
}
</script>
<template>
  <div class="confirm-box">
    <p>确定要执行此操作吗？</p>
    <button @click="handleConfirm" class="btn-confire">确定</button>
    <button @click="handleCancle" class="btn-cancle">取消</button>
  </div>
</template>
