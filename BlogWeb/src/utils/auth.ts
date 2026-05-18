// 解析 JWT 的 Payload 部分（Base64URL 解码）
export function parseToken(token: string): any {
  try {
    const payload = token.split(".")[1];
    // JWT 使用 base64url 编码，需要转换回标准 base64 才能用 atob 解码
    let base64 = payload.replace(/-/g, "+").replace(/_/g, "/");
    // 补齐 padding（base64url 可能缺 padding）
    while (base64.length % 4 !== 0) {
      base64 += "=";
    }
    const decoded = atob(base64);
    return JSON.parse(decoded);
  } catch {
    return null;
  }
}

// 从 payload 中查找 role（兼容 ASP.NET Core 的完整 URI 和其他格式）
function extractRole(payload: any): string | null {
  if (!payload) return null;
  for (const key of Object.keys(payload)) {
    if (key.endsWith("/role")) return payload[key];
  }
  return payload.role || null;
}

// 从 payload 中查找用户名字（兼容多种 claim name 格式）
function extractUsername(payload: any): string | null {
  if (!payload) return null;
  for (const key of Object.keys(payload)) {
    if (key.endsWith("/name")) return payload[key];
  }
  return payload.unique_name || payload.name || null;
}

// 从 payload 中查找用户 ID
function extractUserId(payload: any): number | null {
  if (!payload) return null;
  for (const key of Object.keys(payload)) {
    if (key.endsWith("/nameidentifier")) return parseInt(payload[key], 10);
  }
  if (payload.nameid) return parseInt(payload.nameid, 10);
  return null;
}

// 检查当前用户是否拥有指定角色（区分大小写）
export function hasRole(role: string): boolean {
  const token = localStorage.getItem("token");
  if (!token) return false;
  const userRole = extractRole(parseToken(token));
  return userRole === role;
}

// 获取当前用户名
export function getCurrentUsername(): string | null {
  const token = localStorage.getItem("token");
  if (!token) return null;
  return extractUsername(parseToken(token));
}

// 获取当前用户 ID
export function getCurrentUserId(): number | null {
  const token = localStorage.getItem("token");
  if (!token) return null;
  return extractUserId(parseToken(token));
}
