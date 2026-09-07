// Stores validated authentication data under the existing login keys.
function storeAuthentication({ token, email, fullName }) {
    const normalizedToken =
        typeof token === "string" ? token.trim() : "";

    if (!normalizedToken) {
        throw new Error("Authentication response did not include a token.");
    }

    localStorage.setItem("resume_app_token", normalizedToken);
    localStorage.setItem("user_email", email ?? "");
    localStorage.setItem("user_name", fullName ?? "");
}

export default storeAuthentication;
