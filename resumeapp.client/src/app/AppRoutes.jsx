import { Routes, Route, Link } from "react-router-dom";
import PublicResumePage from "../features/resume/pages/PublicResumePage.jsx";
import MyResumePage from "../features/resume/pages/MyResumePage.jsx";
import LoginPage from "../features/auth/pages/LoginPage.jsx";
import RegisterPage from "../features/auth/pages/RegisterPage.jsx";
import EmailConfirmationPendingPage from "../features/auth/pages/EmailConfirmationPendingPage.jsx";
import EmailConfirmationPage from "../features/auth/pages/EmailConfirmationPage.jsx";
import DashBoardPage from "../features/dashboard/pages/DashBoardPage.jsx";
import ProtectedRoute from "../shared/components/ProtectedRoute.jsx";

function HomePage() {
    const token = localStorage.getItem("resume_app_token");
    const isLoggedIn = Boolean(token);

    return (
        <div className="min-h-screen bg-gradient-to-br from-slate-50 via-blue-50 to-emerald-50 px-4 py-14">
            <section className="mx-auto grid max-w-6xl items-center gap-10 lg:grid-cols-2">
                <div>
                    <div className="mb-5 inline-flex rounded-full border border-blue-200 bg-white px-4 py-2 text-sm font-semibold text-blue-700 shadow-sm">
                        Resume Builder for Developers
                    </div>

                    <h1 className="text-4xl font-extrabold leading-tight tracking-tight text-slate-950 md:text-6xl">
                        Build a resume that looks professional and gets noticed.
                    </h1>

                    <p className="mt-6 max-w-xl text-lg leading-8 text-slate-600">
                        Create, manage, update, and preview your professional resume from one
                        clean dashboard. Built with React, ASP.NET Core, JWT authentication,
                        and SQL Server.
                    </p>

                    <div className="mt-8 flex flex-col gap-4 sm:flex-row">
                        {isLoggedIn ? (
                            <>
                                <Link
                                    to="/dashboard"
                                    className="rounded-2xl bg-blue-600 px-7 py-4 text-center font-bold text-white shadow-lg shadow-blue-200 transition hover:-translate-y-0.5 hover:bg-blue-700"
                                >
                                    Go to Dashboard
                                </Link>

                                <Link
                                    to="/my-resume/preview"
                                    className="rounded-2xl border border-slate-300 bg-white px-7 py-4 text-center font-bold text-slate-800 shadow-sm transition hover:-translate-y-0.5 hover:bg-slate-50"
                                >
                                    Preview My Resume
                                </Link>
                            </>
                        ) : (
                            <>
                                <Link
                                    to="/register"
                                    className="rounded-2xl bg-blue-600 px-7 py-4 text-center font-bold text-white shadow-lg shadow-blue-200 transition hover:-translate-y-0.5 hover:bg-blue-700"
                                >
                                    Get Started
                                </Link>

                                <Link
                                    to="/resume"
                                    className="rounded-2xl border border-slate-300 bg-white px-7 py-4 text-center font-bold text-slate-800 shadow-sm transition hover:-translate-y-0.5 hover:bg-slate-50"
                                >
                                    View Public Resume
                                </Link>
                            </>
                        )}
                    </div>

                    <div className="mt-8 flex flex-wrap gap-3">
                        <span className="rounded-full bg-white px-4 py-2 text-sm font-semibold text-slate-700 shadow-sm">
                            React
                        </span>
                        <span className="rounded-full bg-white px-4 py-2 text-sm font-semibold text-slate-700 shadow-sm">
                            ASP.NET Core
                        </span>
                        <span className="rounded-full bg-white px-4 py-2 text-sm font-semibold text-slate-700 shadow-sm">
                            JWT Auth
                        </span>
                        <span className="rounded-full bg-white px-4 py-2 text-sm font-semibold text-slate-700 shadow-sm">
                            SQL Server
                        </span>
                    </div>
                </div>

                <div className="relative">
                    <div className="absolute -left-6 -top-6 h-24 w-24 rounded-full bg-blue-400 opacity-20 blur-2xl"></div>
                    <div className="absolute -bottom-8 -right-6 h-32 w-32 rounded-full bg-emerald-400 opacity-20 blur-2xl"></div>

                    <div className="relative overflow-hidden rounded-3xl border border-slate-200 bg-white shadow-2xl">
                        <div className="bg-slate-950 px-6 py-7 text-white">
                            <p className="text-sm font-semibold text-emerald-400">
                                LIVE RESUME PREVIEW
                            </p>

                            <h2 className="mt-3 text-3xl font-bold">Future Ibeche</h2>

                            <p className="mt-2 text-lg font-semibold text-blue-300">
                                Full-Stack .NET Developer
                            </p>

                            <p className="mt-3 text-sm text-slate-300">
                                C# | ASP.NET Core | React | SQL Server | Azure
                            </p>
                        </div>

                        <div className="space-y-5 p-6">
                            <div>
                                <h3 className="text-sm font-bold tracking-wide text-slate-900">
                                    SUMMARY
                                </h3>

                                <p className="mt-2 text-sm leading-6 text-slate-600">
                                    Build clean APIs, secure dashboards, and professional resume
                                    pages with modern full-stack tools.
                                </p>
                            </div>

                            <div className="grid gap-4 sm:grid-cols-2">
                                <div className="rounded-2xl bg-blue-50 p-4">
                                    <p className="text-2xl font-extrabold text-blue-700">4+</p>
                                    <p className="text-sm font-medium text-slate-600">
                                        Resume sections
                                    </p>
                                </div>

                                <div className="rounded-2xl bg-emerald-50 p-4">
                                    <p className="text-2xl font-extrabold text-emerald-700">
                                        100%
                                    </p>
                                    <p className="text-sm font-medium text-slate-600">
                                        Auth protected
                                    </p>
                                </div>
                            </div>

                            <div className="rounded-2xl border border-slate-200 p-4">
                                <p className="text-sm font-bold text-slate-900">
                                    Project Highlight
                                </p>

                                <p className="mt-2 text-sm text-slate-600">
                                    CVCrafta — React frontend, ASP.NET Core backend, JWT login,
                                    private dashboard, and public resume preview.
                                </p>
                            </div>
                        </div>
                    </div>
                </div>
            </section>

            <section className="mx-auto mt-14 grid max-w-6xl gap-5 md:grid-cols-3">
                <div className="rounded-3xl border border-slate-200 bg-white p-6 shadow-sm transition hover:-translate-y-1 hover:shadow-lg">
                    <div className="mb-4 flex h-12 w-12 items-center justify-center rounded-2xl bg-blue-100 text-2xl">
                        ✍️
                    </div>

                    <h3 className="text-lg font-bold text-slate-900">
                        Manage your resume
                    </h3>

                    <p className="mt-2 text-sm leading-6 text-slate-600">
                        Update your profile, title, stack, country, and professional summary
                        from your private workspace.
                    </p>
                </div>

                <div className="rounded-3xl border border-slate-200 bg-white p-6 shadow-sm transition hover:-translate-y-1 hover:shadow-lg">
                    <div className="mb-4 flex h-12 w-12 items-center justify-center rounded-2xl bg-emerald-100 text-2xl">
                        🔐
                    </div>

                    <h3 className="text-lg font-bold text-slate-900">
                        Secure dashboard
                    </h3>

                    <p className="mt-2 text-sm leading-6 text-slate-600">
                        JWT authentication protects private resume editing while still
                        allowing a public resume page.
                    </p>
                </div>

                <div className="rounded-3xl border border-slate-200 bg-white p-6 shadow-sm transition hover:-translate-y-1 hover:shadow-lg">
                    <div className="mb-4 flex h-12 w-12 items-center justify-center rounded-2xl bg-purple-100 text-2xl">
                        🚀
                    </div>

                    <h3 className="text-lg font-bold text-slate-900">
                        Ready for deployment
                    </h3>

                    <p className="mt-2 text-sm leading-6 text-slate-600">
                        The project is being prepared for GitHub, CI/CD, Azure deployment,
                        and employer portfolio viewing.
                    </p>
                </div>
            </section>
        </div>
    );
}

function NotFoundPage() {
    return (
        <main className="flex min-h-screen items-center justify-center bg-slate-100 px-6">
            <section className="text-center">
                <h1 className="text-4xl font-bold text-slate-900">404</h1>
                <p className="mt-3 text-slate-600">Page not found.</p>

                <Link
                    to="/"
                    className="mt-6 inline-block rounded-xl bg-blue-600 px-6 py-3 font-semibold text-white transition hover:bg-blue-700"
                >
                    Go Home
                </Link>
            </section>
        </main>
    );
}

// Exposes public registration and one-time confirmation processing routes.
function AppRoutes() {
    return (
        <Routes>
            <Route path="/" element={<HomePage />} />

            <Route path="/resume" element={<PublicResumePage />} />

            <Route path="/login" element={<LoginPage />} />
            <Route path="/register" element={<RegisterPage />} />
            <Route
                path="/confirm-email-pending"
                element={<EmailConfirmationPendingPage />}
            />
            <Route
                path="/confirm-email"
                element={<EmailConfirmationPage />}
            />

            <Route element={<ProtectedRoute />}>
                <Route path="/dashboard" element={<DashBoardPage />} />
                <Route path="/my-resume" element={<MyResumePage />} />
                <Route
                    path="/my-resume/preview"
                    element={<PublicResumePage mode="preview" />}
                />
            </Route>

            <Route path="*" element={<NotFoundPage />} />
        </Routes>
    );
}

export default AppRoutes;
