<script setup lang="ts">
import { ref } from "vue";
import { useRouter } from "vue-router";

const router = useRouter();
const username = ref<string>("");
function login(): void {
  localStorage.setItem("token", "fake-jwt-token");
  alert("登录成功");

  router.push("/");
}
</script>
<template>
  <div style="text-align: center; padding: 50px">
    <h2>登录页面</h2>
    <input v-model="username" placeholder="请输入用户名" />
    <button @click="login">登录</button>
  </div>
</template>
