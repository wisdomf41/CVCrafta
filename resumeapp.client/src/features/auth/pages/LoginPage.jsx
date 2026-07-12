import { useState, useEffect } from "react";
import { Link, useNavigate } from "react-router-dom";
import axiosClient from "../../../core/api/axiosClient";

function LoginPage() {
    const navigate = useNavigate();

    // Redirect if already logged in
    useEffect(() => {
        const token = localStorage.getItem("resume_app_token");
        if (token) {
            navigate("/dashboard");
        }
    }, [navigate]);

    const [formData, setFormData] = useState({
        email: "",
        password: "",
    });

    const [isSubmitting, setIsSubmitting] = useState(false);
    const [error, setError] = useState("");

    const handleChange = (event) => {
        const { name, value } = event.target;
        setFormData((previous) => ({
            ...previous,
            [name]: value,
        }));
    };

    const handleSubmit = async (event) => {
        event.preventDefault();
        setError("");
        setIsSubmitting(true);

        try {
            const response = await axiosClient.post("/auth/login", formData);

            const { token, email, fullName } = response.data;

            if (!token) {
                throw new Error("Login succeeded, but no token was returned.");
            }

            localStorage.setItem("resume_app_token", token);
            localStorage.setItem("user_email", email ?? "");
            localStorage.setItem("user_name", fullName ?? "");

            navigate("/dashboard");
        } catch (error) {
            console.error("Login failed:", error);

            const message =
                error.response?.data?.message ||
                error.response?.data?.title ||
                error.response?.data ||
                "Invalid email or password. Please try again.";

            setError(String(message));
        } finally {
            setIsSubmitting(false);
        }
    };

    return (
        <main className="min-h-screen bg-slate-100 px-6 py-16 flex items-center justify-center">
            <section className="mx-auto max-w-md rounded-2xl bg-white p-8 shadow-sm w-full">
                <div className="text-center">
                    <p className="text-sm font-semibold uppercase tracking-wide text-blue-600">
                        Welcome back
                    </p>
                    <h1 className="mt-3 text-3xl font-bold text-slate-900">Login</h1>
                    <p className="mt-2 text-slate-600">
                        Sign in to manage your resume dashboard.
                    </p>
                </div>

                {error && (
                    <div className="mt-5 rounded-xl border border-red-200 bg-red-50 p-4 text-sm text-red-700">
                        {error}
                    </div>
                )}

                <form onSubmit={handleSubmit} className="mt-6 space-y-5">
                    <div>
                        <label htmlFor="email" className="mb-2 block text-sm font-medium text-slate-700">
                            Email address
                        </label>
                        <input
                            id="email"
                            name="email"
                            type="email"
                            value={formData.email}
                            onChange={handleChange}
                            required
                            className="w-full rounded-xl border border-slate-300 px-4 py-3 outline-none transition focus:border-blue-500 focus:ring-2 focus:ring-blue-100"
                            placeholder="you@example.com"
                        />
                    </div>

                    <div>
                        <label htmlFor="password" className="mb-2 block text-sm font-medium text-slate-700">
                            Password
                        </label>
                        <input
                            id="password"
                            name="password"
                            type="password"
                            value={formData.password}
                            onChange={handleChange}
                            required
                            className="w-full rounded-xl border border-slate-300 px-4 py-3 outline-none transition focus:border-blue-500 focus:ring-2 focus:ring-blue-100"
                            placeholder="Enter your password"
                        />
                    </div>

                    <button
                        type="submit"
                        disabled={isSubmitting}
                        className="w-full rounded-xl bg-blue-600 px-5 py-3 font-semibold text-white transition hover:bg-blue-700 disabled:cursor-not-allowed disabled:bg-blue-300"
                    >
                        {isSubmitting ? "Logging in..." : "Login"}
                    </button>
                </form>

                <p className="mt-6 text-center text-sm text-slate-600">
                    Don&apos;t have an account?{" "}
                    <Link to="/register" className="font-semibold text-blue-600 hover:underline">
                        Register
                    </Link>
                </p>
            </section>
        </main>
    );
}

export default LoginPage;