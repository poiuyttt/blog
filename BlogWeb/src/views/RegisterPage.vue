<script setup lang="ts">
import { ref, reactive } from "vue";
import { useRouter } from "vue-router";
import { ElMessage, type FormInstance, type FormRules } from "element-plus";
import { useAuthStore } from "../stores/auth";
import { register } from "../api/auth";

const router = useRouter();
const authStore = useAuthStore();

const formRef = ref<FormInstance>();

const registerForm = reactive({
  username: "",
  email: "",
  password: "",
  confirmPassword: "",
});

const validateConfirmPassword = (
  rule: any,
  value: string,
  callback: Function,
) => {
  if (value === "") {
    callback(new Error("请再次输入密码"));
  } else if (value !== registerForm.password) {
    callback(new Error("两次输入密码不一致"));
  } else {
    callback();
  }
};

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
  email: [
    { required: true, message: "请输入邮箱", trigger: "blur" },
    { type: "email", message: "请输入正确的邮箱格式", trigger: "blur" },
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
  confirmPassword: [
    { required: true, message: "请再次输入密码", trigger: "blur" },
    { validator: validateConfirmPassword, trigger: "blur" },
  ],
};

const handleRegister = async () => {
  if (!formRef.value) {
    ElMessage.error("请输入用户名、邮箱、密码和确认密码");
    return;
  }
  await formRef.value.validate(async (valid) => {
    if (!valid) {
      ElMessage.error("请填写正确的用户名、邮箱、密码和确认密码");
      return;
    }
    try {
      const res = await register({
        username: registerForm.username,
        email: registerForm.email,
        password: registerForm.password,
      });
      authStore.setAuth(res.data.token, res.data.user);
      ElMessage.success("注册并登录成功！");
      router.push("/");
    } catch (err: any) {
      const msg = err.response?.data?.message || "注册失败";
      ElMessage.error(msg);
    }
  });
};
</script>
<template>
  <div class="auth-container">
    <h2>注册</h2>
    <el-form
      ref="formRef"
      :model="registerForm"
      :rules="rules"
      label-width="100px"
    >
      <el-form-item label="用户名" prop="username">
        <el-input
          v-model="registerForm.username"
          placeholder="请输入用户名"
        ></el-input>
      </el-form-item>
      <el-form-item label="邮箱" prop="email">
        <el-input
          v-model="registerForm.email"
          placeholder="请输入邮箱"
        ></el-input>
      </el-form-item>
      <el-form-item label="密码" prop="password">
        <el-input
          v-model="registerForm.password"
          type="password"
          placeholder="请输入密码"
          show-password
        ></el-input>
      </el-form-item>
      <el-form-item label="确认密码" prop="confirmPassword">
        <el-input
          v-model="registerForm.confirmPassword"
          type="password"
          placeholder="请再次输入密码"
          show-password
        ></el-input>
      </el-form-item>
      <el-form-item>
        <el-button type="primary" @click="handleRegister">注册</el-button>
        <router-link to="/login" class="link">已有帐号？去登录</router-link>
      </el-form-item>
    </el-form>
  </div>
</template>
