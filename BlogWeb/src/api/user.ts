import request from "../utils/request";

export interface UserProfile {
  id: number;
  username: string;
  email: string;
  bio?: string;
  avatar?: string;
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
