import request from "../utils/request";
import type { UserProfile } from "./user";

export function login(data: {
  username: string;
  password: string;
}): Promise<{
  success: boolean;
  data: {
    token: string;
    user: UserProfile;
  };
  message: string;
}> {
  return request.post("/auth/login", data);
}

export function register(data: {
  username: string;
  email: string;
  password: string;
}): Promise<{
  success: boolean;
  data: {
    id: number;
    username: string;
    email: string;
  };
  message: string;
}> {
  return request.post("/auth/register", data);
}
