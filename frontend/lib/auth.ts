// frontend/lib/auth.ts

const API_BASE =
  process.env.NEXT_PUBLIC_BACKEND_URL ?? "http://localhost:5240";

/**
 * Wrapper around fetch that:
 * - Automatically sends JWT
 * - Handles 401 redirects
 */
export async function authFetch(
  url: string,
  options: RequestInit = {}
) {
  const token = typeof window !== "undefined"
    ? localStorage.getItem("token")
    : null;

  const res = await fetch(url, {
    ...options,
    headers: {
      "Content-Type": "application/json",
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
      ...(options.headers || {}),
    },
  });

  if (res.status === 401) {
    if (typeof window !== "undefined") {
      localStorage.removeItem("token");
      window.location.href = "/login";
    }
    throw new Error("Unauthorized");
  }

  return res;
}

/**
 * Save JWT after login/signup
 */
export function setAuthToken(token: string) {
  localStorage.setItem("token", token);
}

/**
 * Logout helper
 */
export function logout() {
  localStorage.removeItem("token");
  window.location.href = "/login";
}
