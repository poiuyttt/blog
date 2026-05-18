<script setup lang="ts">
import { ref, reactive, onMounted } from "vue";
import {
  ElMessage,
  type FormInstance,
  type FormRules,
  type UploadProps,
} from "element-plus";
import { Plus, Loading } from "@element-plus/icons-vue";
import { useAuthStore } from "../stores/auth";
import {
  updateProfile,
  changePassword,
  checkUsername,
  type UserProfile,
} from "../api/user";

const authStore = useAuthStore();
const loading = ref<boolean>(false);
const uploadLoading = ref<boolean>(false);
const passwordLoading = ref<boolean>(false);

const userInfo = ref<UserProfile>({
  id: 0,
  username: "",
  email: "",
  bio: "",
  avatar: "",
  role: "",
});

// 编辑用的表单数据
const editForm = reactive({
  username: "",
  email: "",
  bio: "",
});

// 密码修改表单
const passwordForm = reactive({
  currentPassword: "",
  newPassword: "",
  confirmPassword: "",
});

const isEditing = ref<boolean>(false);
const showPasswordForm = ref<boolean>(false);

// 头像上传地址
const uploadUrl = "https://localhost:7126/api/auth/upload-avatar";

onMounted(() => {
  if (authStore.user) {
    userInfo.value = {
      id: authStore.user.id,
      username: authStore.user.username,
      email: authStore.user.email,
      bio: authStore.user.bio,
      avatar: authStore.user.avatar,
      role: authStore.user.role,
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
    ElMessage.error(err.response?.data?.message || "更新失败");
  } finally {
    loading.value = false;
  }
};

// 头像上传成功回调
const handleAvatarSuccess: UploadProps["onSuccess"] = (response: any) => {
  if (response.success) {
    const userData = response.data;
    authStore.setUser(userData);
    userInfo.value = { ...userData };
    ElMessage.success("头像更新成功");
  } else {
    ElMessage.error(response.message || "上传失败");
  }
  uploadLoading.value = false;
};

// 头像上传失败回调
const handleAvatarError: UploadProps["onError"] = () => {
  ElMessage.error("头像上传失败");
  uploadLoading.value = false;
};

const handleAvatarUpload: UploadProps["beforeUpload"] = (rawFile) => {
  const isImage = rawFile.type.startsWith("image/");
  if (!isImage) {
    ElMessage.error("只能上传图片文件！");
    return false;
  }
  const isLt5M = rawFile.size / 1024 / 1024 < 5;
  if (!isLt5M) {
    ElMessage.error("图片大小不能超过 5MB！");
    return false;
  }
  uploadLoading.value = true;
  return true;
};

// 用户名唯一性验证
const validateUsername = async (rule: any, value: any, callback: any) => {
  if (!value) {
    return callback(new Error("用户名不能为空"));
  }
  if (value.length < 3 || value.length > 20) {
    return callback(new Error("用户名长度必须在 3 到 20 个字符之间"));
  }
  try {
    const res = await checkUsername(value, userInfo.value.id);
    if (res.data.available) {
      return callback();
    } else {
      return callback(new Error("用户名已被占用"));
    }
  } catch (err) {
    return callback(new Error("验证失败"));
  }
};

// 密码确认验证
const validateConfirmPassword = (rule: any, value: any, callback: any) => {
  if (value !== passwordForm.newPassword) {
    callback(new Error("两次输入的密码不一致"));
  } else {
    callback();
  }
};

const formRef = ref<FormInstance>();
const passwordFormRef = ref<FormInstance>();

const rules: FormRules = {
  username: [{ validator: validateUsername, trigger: "blur" }],
  email: [
    { required: true, message: "邮箱不能为空", trigger: "blur" },
    { type: "email", message: "请输入正确的邮箱格式", trigger: "blur" },
  ],
};

const passwordRules: FormRules = {
  currentPassword: [
    { required: true, message: "请输入当前密码", trigger: "blur" },
  ],
  newPassword: [
    { required: true, message: "请输入新密码", trigger: "blur" },
    {
      min: 6,
      max: 50,
      message: "新密码长度必须在 6 到 50 个字符之间",
      trigger: "blur",
    },
  ],
  confirmPassword: [
    { required: true, message: "请确认新密码", trigger: "blur" },
    { validator: validateConfirmPassword, trigger: "blur" },
  ],
};

const startEdit = () => {
  editForm.username = userInfo.value.username;
  editForm.email = userInfo.value.email;
  editForm.bio = userInfo.value.bio || "";
  isEditing.value = true;
};

const cancelEdit = () => {
  isEditing.value = false;
};

const handleChangePassword = async () => {
  if (!passwordFormRef.value) return;
  await passwordFormRef.value.validate(async (valid) => {
    if (valid) {
      passwordLoading.value = true;
      try {
        await changePassword({
          currentPassword: passwordForm.currentPassword,
          newPassword: passwordForm.newPassword,
        });
        ElMessage.success("密码修改成功，请重新登录");
        showPasswordForm.value = false;
        passwordForm.currentPassword = "";
        passwordForm.newPassword = "";
        passwordForm.confirmPassword = "";
        authStore.clearAuth();
      } catch (err: any) {
        ElMessage.error(err.response?.data?.message || "密码修改失败");
      } finally {
        passwordLoading.value = false;
      }
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
        <div class="avatar-wrapper">
          <img
            :src="
              userInfo.avatar ||
              'https://cube.elemecdn.com/0/88/03b0d39583f48206768a7534e55bcpng.png'
            "
            alt="头像"
            class="avatar"
          />
          <div class="avatar-upload" v-if="!isEditing">
            <el-upload
              class="avatar-uploader"
              :show-file-list="false"
              :before-upload="handleAvatarUpload"
              :on-success="handleAvatarSuccess"
              :on-error="handleAvatarError"
              :headers="{ Authorization: `Bearer ${authStore.token}` }"
              :action="uploadUrl"
            >
              <el-icon v-if="!uploadLoading"><Plus /></el-icon>
              <el-icon class="is-loading" v-else><Loading /></el-icon>
            </el-upload>
          </div>
        </div>
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
      <div class="button-group">
        <el-button type="primary" @click="startEdit">编辑资料</el-button>
        <el-button @click="showPasswordForm = true">修改密码</el-button>
      </div>
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
        <el-button type="success" @click="handleSave" :loading="loading">
          保存
        </el-button>
        <el-button @click="cancelEdit">取消</el-button>
      </el-form>
    </div>

    <!-- 修改密码弹窗 -->
    <el-dialog v-model="showPasswordForm" title="修改密码" width="400px">
      <el-form
        ref="passwordFormRef"
        :model="passwordForm"
        :rules="passwordRules"
        label-width="100px"
      >
        <el-form-item label="当前密码" prop="currentPassword">
          <el-input
            v-model="passwordForm.currentPassword"
            type="password"
            placeholder="请输入当前密码"
            show-password
          />
        </el-form-item>
        <el-form-item label="新密码" prop="newPassword">
          <el-input
            v-model="passwordForm.newPassword"
            type="password"
            placeholder="请输入新密码"
            show-password
          />
        </el-form-item>
        <el-form-item label="确认密码" prop="confirmPassword">
          <el-input
            v-model="passwordForm.confirmPassword"
            type="password"
            placeholder="请确认新密码"
            show-password
          />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="showPasswordForm = false">取消</el-button>
        <el-button
          type="primary"
          @click="handleChangePassword"
          :loading="passwordLoading"
        >
          确定
        </el-button>
      </template>
    </el-dialog>
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
.avatar-wrapper {
  position: relative;
}
.avatar {
  width: 80px;
  height: 80px;
  border-radius: 50%;
  object-fit: cover;
}
.avatar-upload {
  position: absolute;
  bottom: 0;
  right: 0;
}
.avatar-uploader :deep(.el-upload) {
  border: 1px solid var(--el-border-color);
  border-radius: 50%;
  cursor: pointer;
  position: relative;
  overflow: hidden;
  transition: var(--el-transition-duration-fast);
}
.avatar-uploader :deep(.el-upload:hover) {
  border-color: var(--el-color-primary);
}
.avatar-uploader :deep(.el-icon) {
  width: 30px;
  height: 30px;
  font-size: 18px;
  color: #fff;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 50%;
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
.button-group {
  margin-top: 20px;
  display: flex;
  gap: 10px;
}
</style>
