<script setup lang="ts">
import { ref, reactive } from "vue";
import { useRouter } from "vue-router";
import { ElMessage, type FormInstance, type FormRules } from "element-plus";

const router = useRouter();

const loginForm = reactive({
  username: "",
  password: "",
});

const formRef = ref<FormInstance>();

const rules: FormRules = {
  username: [
    { required: true, message: "请输入用户名", trigger: "blur" },
    {
      min: 3,
      max: 20,
      message: "用户名长度必须在3到20个字符之间",
      trigger: "blur",
    },
  ],
  password: [
    { required: true, message: "请输入密码", trigger: "blur" },
    {
      min: 6,
      max: 20,
      message: "密码长度必须在6到20个字符之间",
      trigger: "blur",
    },
  ],
};

const handleLogin = async () => {
  if (!formRef.value) {
    return;
  }
  await formRef.value.validate((valid) => {
    if (valid) {
      localStorage.setItem("token", "fake-jwt-token");
      ElMessage.success("登录成功");
      router.push("/");
    } else {
      ElMessage.error("登录失败");
    }
  });
};
</script>
<template>
  <div class="auth-container">
    <h2>登录页面</h2>
    <el-form ref="formRef" :model="loginForm" :rules="rules" label-width="80px">
      <el-form-item label="用户名" prop="username">
        <el-input
          v-model="loginForm.username"
          placeholder="请输入用户名"
        ></el-input>
      </el-form-item>
      <el-form-item label="密码" prop="password">
        <el-input
          v-model="loginForm.password"
          type="password"
          placeholder="请输入密码"
          show-password
        ></el-input>
      </el-form-item>
      <el-form-item>
        <el-button type="primary" @click="handleLogin">登录</el-button>
        <router-link to="/register" class="link">没有账号？去注册</router-link>
      </el-form-item>
    </el-form>
  </div>
</template>
<style scoped>
.auth-container {
  max-width: 400px;
  margin: 50px auto;
  padding: 30px;
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.1);
}
.link {
  margin-left: 15px;
  color: #409eff;
}
</style>
