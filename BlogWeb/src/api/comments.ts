import request from "../utils/request";

export interface Comment {
  id: number;
  postId: number;
  content: string;
  author: string;
  createdAt: string;
}

// 获取指定文章的评论
export function getComments(postId: number): Promise<Comment[]> {
  return request.get("/comment", {
    params: {
      postId,
    },
  });
}

// 发表评论
export function createComment(
  postId: number,
  content: string,
  author?: string,
): Promise<Comment> {
  return request.post("/comment", {
    postId,
    content,
    author,
  });
}

// 删除评论
export function deleteComment(id: number): Promise<void> {
  return request.delete(`/comment/${id}`);
}
