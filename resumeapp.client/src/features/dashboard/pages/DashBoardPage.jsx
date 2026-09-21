import { useNavigate } from "react-router-dom";

function DashBoardPage() {
    const navigate = useNavigate();

    const userName = localStorage.getItem("user_name");
    const userEmail = localStorage.getItem("user_email");

    const displayName = userName || userEmail || "CVCrafta User";

    const userInitial = displayName
        .trim()
        .charAt(0)
        .toUpperCase();

    const handleLogout = () => {
        localStorage.removeItem("resume_app_token");
        localStorage.removeItem("user_email");
        localStorage.removeItem("user_name");

        navigate("/login");
    };

    return (
        <main className="min-h-screen bg-gradient-to-br from-slate-50 via-blue-50 to-emerald-50 px-4 py-10 sm:px-6">
            <section className="mx-auto max-w-6xl">
                {/* Welcome section */}
                <div className="relative overflow-hidden rounded-3xl bg-slate-950 px-6 py-8 text-white shadow-xl sm:px-8 sm:py-10">
                    <div className="absolute -right-16 -top-16 h-52 w-52 rounded-full bg-blue-500/20 blur-3xl" />
                    <div className="absolute -bottom-20 left-1/3 h-52 w-52 rounded-full bg-emerald-500/20 blur-3xl" />

                    <div className="relative flex flex-col gap-6 md:flex-row md:items-center md:justify-between">
                        <div className="flex flex-col gap-5 sm:flex-row sm:items-center">
                            <div className="flex h-16 w-16 shrink-0 items-center justify-center rounded-2xl bg-blue-600 text-2xl font-extrabold shadow-lg shadow-blue-950/30">
                                {userInitial}
                            </div>

                            <div>
                                <p className="text-sm font-semibold uppercase tracking-[0.2em] text-blue-300">
                                    Resume Dashboard
                                </p>

                                <h1 className="mt-2 text-3xl font-extrabold tracking-tight sm:text-4xl">
                                    Welcome back
                                    {userName ? `, ${userName}` : ""}
                                </h1>

                                <p className="mt-3 max-w-2xl text-sm leading-6 text-slate-300 sm:text-base">
                                    Manage your professional information, update your resume, and
                                    preview how it appears before sharing it publicly.
                                </p>

                                {userEmail && (
                                    <p className="mt-3 text-sm font-medium text-slate-400">
                                        Signed in as {userEmail}
                                    </p>
                                )}
                            </div>
                        </div>

                        <button
                            type="button"
                            onClick={handleLogout}
                            className="self-start rounded-xl border border-white/20 bg-white/10 px-5 py-3 font-semibold text-white backdrop-blur transition hover:bg-white/20 md:self-center"
                        >
                            Logout
                        </button>
                    </div>
                </div>

                {/* Quick status cards */}
                <div className="mt-6 grid gap-4 sm:grid-cols-3">
                    <div className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm">
                        <div className="flex items-center gap-3">
                            <div className="flex h-11 w-11 items-center justify-center rounded-xl bg-blue-100 text-xl">
                                📝
                            </div>

                            <div>
                                <p className="text-sm font-medium text-slate-500">
                                    Resume workspace
                                </p>
                                <p className="font-bold text-slate-900">
                                    Ready to update
                                </p>
                            </div>
                        </div>
                    </div>

                    <div className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm">
                        <div className="flex items-center gap-3">
                            <div className="flex h-11 w-11 items-center justify-center rounded-xl bg-emerald-100 text-xl">
                                🔐
                            </div>

                            <div>
                                <p className="text-sm font-medium text-slate-500">
                                    Account status
                                </p>
                                <p className="font-bold text-emerald-700">
                                    Authenticated
                                </p>
                            </div>
                        </div>
                    </div>

                    <div className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm">
                        <div className="flex items-center gap-3">
                            <div className="flex h-11 w-11 items-center justify-center rounded-xl bg-purple-100 text-xl">
                                👁️
                            </div>

                            <div>
                                <p className="text-sm font-medium text-slate-500">
                                    Preview mode
                                </p>
                                <p className="font-bold text-slate-900">
                                    Available
                                </p>
                            </div>
                        </div>
                    </div>
                </div>

                {/* Main dashboard actions */}
                <div className="mt-8">
                    <div className="mb-5">
                        <p className="text-sm font-semibold uppercase tracking-wide text-blue-600">
                            Quick actions
                        </p>

                        <h2 className="mt-1 text-2xl font-bold text-slate-950">
                            What would you like to do?
                        </h2>
                    </div>

                    <div className="grid gap-5 md:grid-cols-2">
                        {/* Manage resume */}
                        <article className="group rounded-3xl border border-slate-200 bg-white p-6 shadow-sm transition hover:-translate-y-1 hover:shadow-xl">
                            <div className="flex h-14 w-14 items-center justify-center rounded-2xl bg-blue-100 text-2xl transition group-hover:scale-105">
                                ✍️
                            </div>

                            <h3 className="mt-5 text-xl font-bold text-slate-950">
                                Manage My Resume
                            </h3>

                            <p className="mt-2 leading-7 text-slate-600">
                                View and update your name, professional title, technology
                                stack, location, contact details, and professional summary.
                            </p>

                            <button
                                type="button"
                                onClick={() => navigate("/my-resume")}
                                className="mt-6 w-full rounded-xl bg-blue-600 px-5 py-3 font-semibold text-white shadow-sm transition hover:bg-blue-700 sm:w-auto"
                            >
                                Open Resume Editor
                            </button>
                        </article>

                        {/* Preview resume */}
                        <article className="group rounded-3xl border border-slate-200 bg-white p-6 shadow-sm transition hover:-translate-y-1 hover:shadow-xl">
                            <div className="flex h-14 w-14 items-center justify-center rounded-2xl bg-emerald-100 text-2xl transition group-hover:scale-105">
                                👁️
                            </div>

                            <h3 className="mt-5 text-xl font-bold text-slate-950">
                                Preview Updated Resume
                            </h3>

                            <p className="mt-2 leading-7 text-slate-600">
                                See how your saved resume currently appears before making it
                                available to employers or sharing it with other people.
                            </p>

                            <button
                                type="button"
                                onClick={() => navigate("/my-resume/preview")}
                                className="mt-6 w-full rounded-xl bg-emerald-600 px-5 py-3 font-semibold text-white shadow-sm transition hover:bg-emerald-700 sm:w-auto"
                            >
                                Preview My Resume
                            </button>
                        </article>

                        {/* General public resume */}
                        <article className="group rounded-3xl border border-slate-200 bg-white p-6 shadow-sm transition hover:-translate-y-1 hover:shadow-xl">
                            <div className="flex h-14 w-14 items-center justify-center rounded-2xl bg-purple-100 text-2xl transition group-hover:scale-105">
                                🌍
                            </div>

                            <h3 className="mt-5 text-xl font-bold text-slate-950">
                                View Public Resume
                            </h3>

                            <p className="mt-2 leading-7 text-slate-600">
                                Open the general public resume page that visitors can access
                                without signing into the application.
                            </p>

                            <button
                                type="button"
                                onClick={() => navigate("/resume")}
                                className="mt-6 w-full rounded-xl border border-slate-300 bg-white px-5 py-3 font-semibold text-slate-700 transition hover:bg-slate-50 sm:w-auto"
                            >
                                Open Public Page
                            </button>
                        </article>

                        {/* Home */}
                        <article className="group rounded-3xl border border-slate-200 bg-white p-6 shadow-sm transition hover:-translate-y-1 hover:shadow-xl">
                            <div className="flex h-14 w-14 items-center justify-center rounded-2xl bg-amber-100 text-2xl transition group-hover:scale-105">
                                🏠
                            </div>

                            <h3 className="mt-5 text-xl font-bold text-slate-950">
                                Return to Homepage
                            </h3>

                            <p className="mt-2 leading-7 text-slate-600">
                                Return to the CVCrafta landing page and explore the public
                                features and project information.
                            </p>

                            <button
                                type="button"
                                onClick={() => navigate("/")}
                                className="mt-6 w-full rounded-xl border border-slate-300 bg-white px-5 py-3 font-semibold text-slate-700 transition hover:bg-slate-50 sm:w-auto"
                            >
                                Go to Homepage
                            </button>
                        </article>
                    </div>
                </div>

                {/* Dashboard note */}
                <div className="mt-8 rounded-2xl border border-blue-200 bg-blue-50 p-5">
                    <div className="flex items-start gap-3">
                        <span className="text-xl">💡</span>

                        <div>
                            <p className="font-bold text-blue-950">
                                Keep your resume current
                            </p>

                            <p className="mt-1 text-sm leading-6 text-blue-800">
                                Update your resume whenever you complete a project, earn a
                                certification, learn a new skill, or change your professional
                                focus.
                            </p>
                        </div>
                    </div>
                </div>
            </section>
        </main>
    );
}

export default DashBoardPage;