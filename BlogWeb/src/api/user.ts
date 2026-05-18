import request from "../utils/request";

export interface UserProfile {
  id: number;
  username: string;
  email: string;
  bio?: string;
  avatar?: string;
  role: string;
}

export function updateProfile(data: {
  username: string;
  email: string;
  bio?: string;
}): Promise<{
  success: boolean;
  data: UserProfile;
  message: string;
}> {
  return request.put("/auth/profile", data);
}

export function changePassword(data: {
  currentPassword: string;
  newPassword: string;
}): Promise<{
  success: boolean;
  message: string;
}> {
  return request.put("/auth/change-password", data);
}

export function checkUsername(
  username: string,
  excludeUserId?: number,
): Promise<{
  success: boolean;
  data: { available: boolean };
  message: string;
}> {
  const params: any = { username };
  if (excludeUserId) {
    params.excludeUserId = excludeUserId;
  }
  return request.get("/auth/check-username", { params });
}
