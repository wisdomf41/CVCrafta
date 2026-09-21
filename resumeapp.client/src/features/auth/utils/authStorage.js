// Centralizes authentication keys, storage, and validation for cross-tab use.
export const AUTH_STORAGE_KEYS = Object.freeze({
    token: "resume_app_token",
    email: "user_email",
    fullName: "user_name",
});

const jwtPattern = /^[A-Za-z0-9_-]+\.[A-Za-z0-9_-]+\.[A-Za-z0-9_-]+$/;
const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

export function isAuthenticationStorageKey(key) {
    return Object.values(AUTH_STORAGE_KEYS).includes(key);
}

export function hasValidAuthenticationStorage(storage = localStorage) {
    const token = storage.getItem(AUTH_STORAGE_KEYS.token)?.trim() ?? "";
    const email = storage.getItem(AUTH_STORAGE_KEYS.email)?.trim() ?? "";
    const fullName = storage.getItem(AUTH_STORAGE_KEYS.fullName)?.trim() ?? "";

    return (
        jwtPattern.test(token) &&
        emailPattern.test(email) &&
        fullName.length > 0
    );
}

function storeAuthentication({ token, email, fullName }) {
    const normalizedToken =
        typeof token === "string" ? token.trim() : "";

    if (!normalizedToken) {
        throw new Error("Authentication response did not include a token.");
    }

    localStorage.setItem(AUTH_STORAGE_KEYS.token, normalizedToken);
    localStorage.setItem(
        AUTH_STORAGE_KEYS.email,
        typeof email === "string" ? email : ""
    );
    localStorage.setItem(
        AUTH_STORAGE_KEYS.fullName,
        typeof fullName === "string" ? fullName : ""
    );
}

export default storeAuthentication;
