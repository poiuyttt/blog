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
  categoryName?: string;
}

export interface Category {
  id: number;
  name: string;
  postCount: number;
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
  pageSize: number = 5,
): Promise<{
  success: boolean;
  data: {
    totalCount: number;
    page: number;
    pageSize: number;
    totalPages: number;
    data: Post[];
  };
}> {
  return request.get("/post/paged", { params: { page, pageSize } });
}

export function createPost(data: {
  title: string;
  content: string;
  summary?: string;
  categoryId?: number;
}): Promise<{
  success: boolean;
  data: Post;
  message: string;
}> {
  return request.post("/post", data);
}

export function updatePost(
  id: number,
  data: {
    title: string;
    content: string;
    summary?: string;
    categoryId?: number;
  },
): Promise<{
  success: boolean;
  data: Post;
  message: string;
}> {
  return request.put(`/post/${id}`, data);
}

export function searchPosts(
  keyword: string = "",
  categoryId?: number,
  page: number = 1,
  pageSize: number = 10,
): Promise<{
  success: boolean;
  data: {
    totalCount: number;
    page: number;
    pageSize: number;
    totalPages: number;
    data: Post[];
  };
  message: string;
}> {
  const params: any = { keyword, page, pageSize };
  if (categoryId) params.categoryId = categoryId;
  return request.get("/post/search", { params });
}

export function getCategories(): Promise<{
  success: boolean;
  data: Category[];
  message: string;
}> {
  return request.get("/category");
}

export function createCategory(data: { name: string }): Promise<{
  success: boolean;
  data: Category;
  message: string;
}> {
  return request.post("/category", data);
}
