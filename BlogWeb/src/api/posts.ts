import request from "../utils/request";

export interface Post {
  id: number;
  title: string;
  content: string;
  summary?: string;
  author: string;
  createdAt: string;
  updatedAt?: string;
  commentCount: number;
}

export function getPosts(): Promise<Post[]> {
  return request.get("/post");
}

export function getPostById(id: number): Promise<{
  success: boolean;
  data: Post;
  message: string;
}> {
  return request.get(`/post/${id}`);
}

export function getPagedPosts(
  page: number = 1,
  pagesize: number = 5,
): Promise<{
  success: boolean;
  data: {
    totalCount: number;
    page: number;
    pagesize: number;
    totalPage: number;
    data: Post[];
  };
}> {
  return request.get("/post/paged", { params: { page, pagesize } });
}
