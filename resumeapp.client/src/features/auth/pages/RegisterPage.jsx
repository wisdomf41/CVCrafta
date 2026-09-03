import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import axiosClient from "../../../core/api/axiosClient";

function RegisterPage() {
    const navigate = useNavigate();

    const [formData, setFormData] = useState({
        fullName: "",
        email: "",
        password: "",
        confirmPassword: "",
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

    // Carries only the registered email into the confirmation-pending experience.
    const handleSubmit = async (event) => {
        event.preventDefault();

        setError("");

        if (formData.password !== formData.confirmPassword) {
            setError("Passwords do not match.");
            return;
        }

        if (formData.password.length < 6) {
            setError("Password must be at least 6 characters.");
            return;
        }

        setIsSubmitting(true);

        try {
            const registeredEmail = formData.email.trim();

            await axiosClient.post("/auth/register", {
                fullName: formData.fullName.trim(),
                email: registeredEmail,
                password: formData.password,
            });

            setFormData((previous) => ({
                ...previous,
                password: "",
                confirmPassword: "",
            }));

            navigate("/confirm-email-pending", {
                state: { email: registeredEmail },
            });
        } catch (error) {
            let message =
                error.response?.data?.message ||
                error.response?.data?.title ||
                error.response?.data ||
                error.message ||
                "Registration failed. Please try again.";

            if (
                typeof message === "string" &&
                message.toLowerCase().includes("user already exists")
            ) {
                message =
                    "This email is already registered. Please sign in instead.";
            }

            setError(String(message));
        } finally {
            setIsSubmitting(false);
        }
    };

    return (
        <main className="min-h-[calc(100vh-80px)] bg-gradient-to-br from-slate-50 via-blue-50 to-emerald-50 px-4 py-12 sm:px-6">
            <section className="mx-auto grid max-w-6xl overflow-hidden rounded-3xl border border-slate-200 bg-white shadow-2xl lg:grid-cols-2">
                {/* Information panel */}
                <div className="relative hidden overflow-hidden bg-slate-950 p-10 text-white lg:flex lg:flex-col lg:justify-between">
                    <div className="absolute -left-20 -top-20 h-64 w-64 rounded-full bg-blue-500/20 blur-3xl" />
                    <div className="absolute -bottom-24 -right-16 h-72 w-72 rounded-full bg-emerald-500/20 blur-3xl" />

                    <div className="relative">
                        <div className="inline-flex rounded-full border border-white/10 bg-white/10 px-4 py-2 text-sm font-semibold text-blue-200">
                            ResumeApp Workspace
                        </div>

                        <h1 className="mt-7 text-4xl font-extrabold leading-tight tracking-tight">
                            Create your professional resume workspace.
                        </h1>

                        <p className="mt-5 max-w-lg leading-7 text-slate-300">
                            Create an account to manage your resume, update your professional
                            information, and preview how your profile appears to employers.
                        </p>
                    </div>

                    <div className="relative mt-10 space-y-4">
                        <div className="flex items-start gap-4 rounded-2xl border border-white/10 bg-white/5 p-4">
                            <div className="flex h-11 w-11 shrink-0 items-center justify-center rounded-xl bg-blue-500/20 text-xl">
                                ✍️
                            </div>

                            <div>
                                <p className="font-bold">Build your resume</p>
                                <p className="mt-1 text-sm leading-6 text-slate-400">
                                    Add your profile, skills, projects, experience, education,
                                    and professional summary.
                                </p>
                            </div>
                        </div>

                        <div className="flex items-start gap-4 rounded-2xl border border-white/10 bg-white/5 p-4">
                            <div className="flex h-11 w-11 shrink-0 items-center justify-center rounded-xl bg-emerald-500/20 text-xl">
                                🔐
                            </div>

                            <div>
                                <p className="font-bold">Private account access</p>
                                <p className="mt-1 text-sm leading-6 text-slate-400">
                                    Your resume editor and dashboard remain protected behind your
                                    secure account.
                                </p>
                            </div>
                        </div>

                        <div className="flex items-start gap-4 rounded-2xl border border-white/10 bg-white/5 p-4">
                            <div className="flex h-11 w-11 shrink-0 items-center justify-center rounded-xl bg-purple-500/20 text-xl">
                                👁️
                            </div>

                            <div>
                                <p className="font-bold">Preview before sharing</p>
                                <p className="mt-1 text-sm leading-6 text-slate-400">
                                    Review your saved resume before making it visible to
                                    employers or other visitors.
                                </p>
                            </div>
                        </div>
                    </div>
                </div>

                {/* Registration form */}
                <div className="flex items-center px-6 py-10 sm:px-10 lg:px-12">
                    <div className="mx-auto w-full max-w-md">
                        <div className="lg:hidden">
                            <div className="inline-flex rounded-full bg-blue-50 px-4 py-2 text-sm font-semibold text-blue-700">
                                ResumeApp Workspace
                            </div>
                        </div>

                        <p className="mt-5 text-sm font-semibold uppercase tracking-[0.2em] text-blue-600 lg:mt-0">
                            Get started
                        </p>

                        <h2 className="mt-3 text-3xl font-extrabold tracking-tight text-slate-950 sm:text-4xl">
                            Create your account
                        </h2>

                        <p className="mt-3 leading-7 text-slate-600">
                            Enter your details to create your private resume workspace.
                        </p>

                        {error && (
                            <div
                                role="alert"
                                className="mt-6 rounded-2xl border border-red-200 bg-red-50 p-4 text-sm text-red-700"
                            >
                                {error}
                            </div>
                        )}

                        <form onSubmit={handleSubmit} className="mt-7 space-y-5">
                            <div>
                                <label
                                    htmlFor="fullName"
                                    className="mb-2 block text-sm font-semibold text-slate-700"
                                >
                                    Full name
                                </label>

                                <input
                                    id="fullName"
                                    name="fullName"
                                    type="text"
                                    value={formData.fullName}
                                    onChange={handleChange}
                                    required
                                    autoComplete="name"
                                    placeholder="e.g. Amanda Miller"
                                    className="w-full rounded-xl border border-slate-300 bg-white px-4 py-3.5 text-slate-900 outline-none transition placeholder:text-slate-400 focus:border-blue-500 focus:ring-4 focus:ring-blue-100"
                                />
                            </div>

                            <div>
                                <label
                                    htmlFor="email"
                                    className="mb-2 block text-sm font-semibold text-slate-700"
                                >
                                    Email address
                                </label>

                                <input
                                    id="email"
                                    name="email"
                                    type="email"
                                    value={formData.email}
                                    onChange={handleChange}
                                    required
                                    autoComplete="email"
                                    placeholder="you@example.com"
                                    className="w-full rounded-xl border border-slate-300 bg-white px-4 py-3.5 text-slate-900 outline-none transition placeholder:text-slate-400 focus:border-blue-500 focus:ring-4 focus:ring-blue-100"
                                />
                            </div>

                            <div>
                                <div className="mb-2 flex items-center justify-between gap-3">
                                    <label
                                        htmlFor="password"
                                        className="block text-sm font-semibold text-slate-700"
                                    >
                                        Password
                                    </label>

                                    <span className="text-xs font-medium text-slate-400">
                                        Minimum 6 characters
                                    </span>
                                </div>

                                <input
                                    id="password"
                                    name="password"
                                    type="password"
                                    value={formData.password}
                                    onChange={handleChange}
                                    required
                                    minLength={6}
                                    autoComplete="new-password"
                                    placeholder="Create a secure password"
                                    className="w-full rounded-xl border border-slate-300 bg-white px-4 py-3.5 text-slate-900 outline-none transition placeholder:text-slate-400 focus:border-blue-500 focus:ring-4 focus:ring-blue-100"
                                />
                            </div>

                            <div>
                                <label
                                    htmlFor="confirmPassword"
                                    className="mb-2 block text-sm font-semibold text-slate-700"
                                >
                                    Confirm password
                                </label>

                                <input
                                    id="confirmPassword"
                                    name="confirmPassword"
                                    type="password"
                                    value={formData.confirmPassword}
                                    onChange={handleChange}
                                    required
                                    minLength={6}
                                    autoComplete="new-password"
                                    placeholder="Repeat your password"
                                    className="w-full rounded-xl border border-slate-300 bg-white px-4 py-3.5 text-slate-900 outline-none transition placeholder:text-slate-400 focus:border-blue-500 focus:ring-4 focus:ring-blue-100"
                                />
                            </div>

                            <button
                                type="submit"
                                disabled={isSubmitting}
                                className="w-full rounded-xl bg-blue-600 px-5 py-3.5 font-bold text-white shadow-lg shadow-blue-200 transition hover:-translate-y-0.5 hover:bg-blue-700 disabled:cursor-not-allowed disabled:translate-y-0 disabled:bg-blue-300 disabled:shadow-none"
                            >
                                {isSubmitting ? "Creating account..." : "Create account"}
                            </button>
                        </form>

                        <div className="mt-7 border-t border-slate-200 pt-6 text-center">
                            <p className="text-sm text-slate-600">
                                Already have an account?{" "}
                                <Link
                                    to="/login"
                                    className="font-bold text-blue-600 transition hover:text-blue-700 hover:underline"
                                >
                                    Sign in
                                </Link>
                            </p>
                        </div>

                        <div className="mt-6 rounded-2xl bg-slate-50 p-4">
                            <p className="text-center text-xs leading-5 text-slate-500">
                                By creating an account, you gain access to your private resume
                                editor, dashboard, and personal resume preview.
                            </p>
                        </div>
                    </div>
                </div>
            </section>
        </main>
    );
}

export default RegisterPage;
