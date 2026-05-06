import request from "../utils/request";

export interface Post {
  id: number;
  title: string;
  content: string;
  author: string;
  createdAt: string;
}

export function getPosts(): Promise<Post[]> {
  return request.get("/posts");
}

export function getPostById(id: number): Promise<Post> {
  return request.get(`/posts/${id}`);
}
