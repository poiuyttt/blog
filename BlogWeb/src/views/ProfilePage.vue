<script setup lang="ts">
import { ref, reactive } from "vue";
import { ElMessage, type FormInstance, type FormRules } from "element-plus";

// 用户信息（模拟数据，后续从 Pinia Store 或 API 获取）
const uesrInfo = reactive({
  username: "张三",
  email: "zhangsan@example.com",
  avatar: "https://picsum.photos/100/100?random=1",
  bio: "全栈开发学习者，热爱 C# 和 Vue3。",
  createdAt: "2026-04-20",
});

// 编辑模式开关
const isEditing = ref<boolean>(false);

// 表单 ref 实例
const formRef = ref<FormInstance>();

// 编辑用的表单数据（复制一份，避免直接修改原数据）
const editForm = reactive({
  username: "",
  email: "",
  bio: "",
});

// 表单验证规则
const rules: FormRules = {
  username: [
    { required: true, message: "用户名不能为空", trigger: "blur" },
    {
      min: 3,
      max: 20,
      message: "用户名长度必须在 3 到 20 个字符之间",
      trigger: "blur",
    },
  ],
  email: [
    { required: true, message: "邮箱不能为空", trigger: "blur" },
    { type: "email", message: "请输入正确的邮箱邮箱格式", trigger: "blur" },
  ],
};

// 进入编辑模式
const startEdit = () => {
  editForm.username = uesrInfo.username;
  editForm.email = uesrInfo.email;
  editForm.bio = uesrInfo.bio;
  isEditing.value = true;
};

//取消编辑模式
const cancelEdit = () => {
  isEditing.value = false;
};

//保存修改
const handleSave = async () => {
  if (!formRef.value) return;
  // validate：执行表单验证
  await formRef.value.validate((valid) => {
    if (valid) {
      uesrInfo.username = editForm.username;
      uesrInfo.email = editForm.email;
      uesrInfo.bio = editForm.bio;
      isEditing.value = false;
      ElMessage.success("修改成功");
    } else {
      ElMessage.error("请填写完整信息");
    }
  });
};
</script>

<template>
  <div class="profile-page">
    <h2>个人中心</h2>

    <!-- 用户信息卡片 -->
    <el-card class="profile-card">
      <div class="profile-header">
        <img :src="uesrInfo.avatar" alt="头像" class="avatar" />
        <div class="basic-info">
          <h3>{{ uesrInfo.username }}</h3>
          <p class="join-date">加入于：{{ uesrInfo.createdAt }}</p>
        </div>
      </div>
    </el-card>

    <!-- 查看模式 -->
    <div v-if="!isEditing" class="view-mode">
      <div class="info-item">
        <span class="label">邮箱：</span>
        <span>{{ uesrInfo.email }}</span>
      </div>
      <div class="info-item">
        <span class="label">个人简介：</span>
        <span>{{ uesrInfo.bio || "暂无简介" }}</span>
      </div>
      <el-button type="primary" @click="startEdit">编辑资料 </el-button>
    </div>

    <!-- 编辑模式 -->
    <div v-else class="edit-mode">
      <el-form
        ref="formRef"
        :model="editForm"
        :rules="rules"
        label-width="80px"
      >
        <el-form-item label="用户名" prop="username">
          <el-input v-model="editForm.username" placeholder="请输入用户名" />
        </el-form-item>
        <el-form-item label="邮箱" prop="email">
          <el-input v-model="editForm.email" placeholder="请输入邮箱" />
        </el-form-item>
        <el-form-item label="个人简介" prop="bio">
          <el-input
            v-model="editForm.bio"
            type="textarea"
            rows="3"
            placeholder="请输入个人简介"
          />
        </el-form-item>
        <el-button type="success" @click="handleSave">保存</el-button>
        <el-button @click="cancelEdit">取消</el-button>
      </el-form>
    </div>
  </div>
</template>

<style scoped>
.profile-page {
  max-width: 600px;
  margin: 0 auto;
  padding: 20px;
}
.profile-header {
  display: flex;
  align-items: center;
  gap: 20px;
}
.avatar {
  width: 80px;
  height: 80px;
  border-radius: 50%;
  object-fit: cover;
}
.basic-info h3 {
  margin: 0 0 5px 0;
}
.join-date {
  color: #999;
  font-size: 13px;
  margin: 0;
}
.info-item {
  margin-bottom: 12px;
}
.label {
  color: #888;
  font-weight: bold;
}
</style>
