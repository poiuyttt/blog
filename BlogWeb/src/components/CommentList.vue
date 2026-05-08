<script setup lang="ts">
import { ref } from "vue";

interface Comment {
  id: number;
  author: string;
  content: string;
  createdAt: string;
}

interface Props {
  comments: Comment[];
  loading?: boolean;
}
const props = withDefaults(defineProps<Props>(), {
  loading: false,
});

// ----- Emits：向父组件发送事件 -----
const emit = defineEmits<{
  // refresh：请求刷新评论列表
  (event: "refresh"): void;
  // delete：请求删除某条评论，参数为评论 ID
  (event: "delete", id: number): void;
}>();

// 新评论内容
const newComment = ref<string>("");

// 提交新评论
const handleSubmit = () => {
  if (!newComment.value.trim()) return;
  // 先 emit 通知父组件，由父组件处理实际提交逻辑
  emit("refresh");
  newComment.value = "";
  alert("评论提交功能将在下周接入API");
};
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
            @click="emit('delete', comment.id)"
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
