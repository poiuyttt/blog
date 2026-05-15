<script setup lang="ts">
import { ref, onMounted } from "vue";
import { ElMessage } from "element-plus";
import { useAuthStore } from "../stores/auth";
import {
  getComments,
  createComment,
  deleteComment,
  type Comment,
} from "../api/comments";

interface Props {
  postId: number;
}

const props = defineProps<Props>();
const authStore = useAuthStore();

const comments = ref<Comment[]>([]);
const loading = ref<boolean>(false);
const newComment = ref<string>("");

// ----- Emits：向父组件发送事件 -----
const emit = defineEmits<{
  // refresh：请求刷新评论列表
  (event: "refresh"): void;
  // delete：请求删除某条评论，参数为评论 ID
  (event: "delete", id: number): void;
}>();

// 加载评论
const loadComments = async () => {
  loading.value = true;
  try {
    comments.value = await getComments(props.postId);
  } catch (err) {
    ElMessage.error("加载评论失败");
  } finally {
    loading.value = false;
  }
};

// 提交评论
const handleSubmit = async () => {
  if (!newComment.value.trim()) return;
  try {
    await createComment(
      props.postId,
      newComment.value,
      authStore.user?.username,
    );
    newComment.value = "";
    ElMessage.success("评论发表成功");
    await loadComments(); // 重新加载评论列表
  } catch (err) {
    ElMessage.error("评论发表失败");
  }
};

// 删除评论
const handleDelete = async (id: number) => {
  try {
    await deleteComment(id);
    ElMessage.success("评论已删除");
    await loadComments();
  } catch (err) {
    ElMessage.error("删除失败");
  }
};

onMounted(() => {
  loadComments();
});
</script>

<template>
  <div class="comment-section">
    <!-- 加载中 -->
    <div v-if="loading" class="loading">
      <el-skeleton :rows="3" animated />
    </div>

    <!-- 评论列表 -->
    <div v-else>
      <div v-if="comments.length === 0" class="empty">
        暂无评论，快来发表第一条评论吧
      </div>
      <div v-for="comment in comments" :key="comment.id" class="comment-item">
        <div class="comment-header">
          <span class="author">{{ comment.author }}</span>
          <span class="time">{{ comment.createdAt }}</span>
        </div>
        <p class="comment-content">{{ comment.content }}</p>
        <div class="comment-actions">
          <el-button
            type="danger"
            size="small"
            @click="handleDelete(comment.id)"
          >
            删除
          </el-button>
        </div>
      </div>
    </div>
    <!-- 评论输入框 -->
    <div class="comment-input">
      <el-input
        v-model="newComment"
        type="textarea"
        :rows="3"
        placeholder="写下你的评论..."
      />
      <el-button type="primary" class="submit-btn" @click="handleSubmit">
        发表评论
      </el-button>
    </div>
  </div>
</template>

<style scoped>
.comment-section {
  margin-top: 30px;
  background: white;
  border-radius: 8px;
  padding: 20px;
}
.comment-item {
  border-bottom: 1px solid #eee;
  padding: 15px 0;
}
.comment-header {
  display: flex;
  justify-content: space-between;
  margin-bottom: 8px;
}
.author {
  font-weight: bold;
  color: #333;
}
.time {
  font-size: 12px;
  color: #999;
}
.comment-content {
  color: #555;
  line-height: 1.6;
}
.comment-actions {
  margin-top: 8px;
}
.empty {
  color: #999;
  text-align: center;
  padding: 30px;
}
.comment-input {
  margin-top: 20px;
}
.submit-btn {
  margin-top: 10px;
}
</style>
