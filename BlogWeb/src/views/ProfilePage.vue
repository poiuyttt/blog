<script setup lang="ts">
import { ref, reactive, onMounted } from "vue";
import { ElMessage, type FormInstance, type FormRules } from "element-plus";
import { useAuthStore } from "../stores/auth";
import { updateProfile, type UserProfile } from "../api/user";

const authStore = useAuthStore();
const loading = ref<boolean>(false);

const userInfo = ref<UserProfile>({
  id: 0,
  username: "",
  email: "",
  bio: "",
});

// 编辑用的表单数据（复制一份，避免直接修改原数据）
const editForm = reactive({
  username: "",
  email: "",
  bio: "",
});

const isEditing = ref<boolean>(false);

onMounted(() => {
  if (authStore.user) {
    userInfo.value = {
      id: authStore.user.id,
      username: authStore.user.username,
      email: authStore.user.email,
      bio: authStore.user.bio,
    };
    editForm.username = userInfo.value.username;
    editForm.email = userInfo.value.email;
    editForm.bio = userInfo.value.bio || "";
  }
});

const handleSave = async () => {
  loading.value = true;
  try {
    const res = await updateProfile(editForm);
    authStore.setUser(res.data);
    userInfo.value = { ...res.data };
    isEditing.value = false;
    ElMessage.success("个人信息已更新");
  } catch (err: any) {
    ElMessage.error(err.message?.data?.message || "更新失败");
  } finally {
    loading.value = false;
  }
};

// 表单 ref 实例
const formRef = ref<FormInstance>();

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
  editForm.username = userInfo.value.username;
  editForm.email = userInfo.value.email;
  editForm.bio = userInfo.value.bio || "";
  isEditing.value = true;
};

//取消编辑模式
const cancelEdit = () => {
  isEditing.value = false;
};
</script>

<template>
  <div class="profile-page">
    <h2>个人中心</h2>

    <!-- 用户信息卡片 -->
    <el-card class="profile-card">
      <div class="profile-header">
        <img :src="userInfo.avatar" alt="头像" class="avatar" />
        <div class="basic-info">
          <h3>{{ userInfo.username }}</h3>
        </div>
      </div>
    </el-card>

    <!-- 查看模式 -->
    <div v-if="!isEditing" class="view-mode">
      <div class="info-item">
        <span class="label">邮箱：</span>
        <span>{{ userInfo.email }}</span>
      </div>
      <div class="info-item">
        <span class="label">个人简介：</span>
        <span>{{ userInfo.bio || "暂无简介" }}</span>
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
