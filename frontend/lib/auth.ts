export function getToken(): string | null {
  if (typeof window === "undefined") return null;
  return localStorage.getItem("token");
}

export function isLoggedIn(): boolean {
  return !!getToken();
}

export function logout() {
  localStorage.removeItem("token");
  window.location.href = "/";
}

/**
 * Fetch helper that automatically attaches Authorization header
 */
export async function authFetch(
  input: RequestInfo,
  init: RequestInit = {}
) {
  const token = getToken();

  return fetch(input, {
    ...init,
    headers: {
      ...(init.headers || {}),
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
    },
  });
}
