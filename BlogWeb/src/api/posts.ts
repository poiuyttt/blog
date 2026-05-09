import request from "../utils/request";

export interface Post {
  id: number;
  title: string;
  content: string;
  summary?: string;
  author: string;
  createdAt: string;
  updatedAt?: string;
}

export function getPosts(): Promise<Post[]> {
  return request.get("/post");
}

export function getPostById(id: number): Promise<Post> {
  return request.get(`/post/${id}`);
}
