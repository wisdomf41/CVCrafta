import { Link } from "react-router-dom";

function AppFooter() {
    const currentYear = new Date().getFullYear();

    return (
        <footer className="border-t border-slate-200 bg-slate-950 px-4 py-10 text-slate-300">
            <div className="mx-auto max-w-6xl">
                <div className="grid gap-8 md:grid-cols-3">
                    {/* Brand */}
                    <div>
                        <Link to="/" className="inline-flex items-center gap-3">
                            <div className="flex h-11 w-11 items-center justify-center rounded-2xl bg-blue-600 text-lg font-extrabold text-white">
                                R
                            </div>

                            <div>
                                <p className="text-xl font-extrabold text-white">
                                    ResumeApp
                                </p>

                                <p className="text-xs font-medium text-slate-400">
                                    Build. Manage. Share.
                                </p>
                            </div>
                        </Link>

                        <p className="mt-4 max-w-sm text-sm leading-6 text-slate-400">
                            A modern full-stack resume platform for creating, updating,
                            previewing, and sharing professional resumes.
                        </p>
                    </div>

                    {/* Navigation */}
                    <div>
                        <h3 className="font-bold text-white">
                            Quick Links
                        </h3>

                        <div className="mt-4 flex flex-col gap-3 text-sm">
                            <Link
                                to="/"
                                className="transition hover:text-white"
                            >
                                Home
                            </Link>

                            <Link
                                to="/resume"
                                className="transition hover:text-white"
                            >
                                Public Resume
                            </Link>

                            <Link
                                to="/login"
                                className="transition hover:text-white"
                            >
                                Login
                            </Link>

                            <Link
                                to="/register"
                                className="transition hover:text-white"
                            >
                                Create Account
                            </Link>
                        </div>
                    </div>

                    {/* Technology */}
                    <div>
                        <h3 className="font-bold text-white">
                            Built With
                        </h3>

                        <div className="mt-4 flex flex-wrap gap-2">
                            {[
                                "React",
                                "ASP.NET Core",
                                "SQL Server",
                                "JWT",
                                "Tailwind CSS",
                            ].map((technology) => (
                                <span
                                    key={technology}
                                    className="rounded-full border border-slate-700 bg-slate-900 px-3 py-1.5 text-xs font-semibold text-slate-300"
                                >
                                    {technology}
                                </span>
                            ))}
                        </div>

                        <p className="mt-4 text-sm leading-6 text-slate-400">
                            Designed as a production-ready portfolio project with secure
                            authentication and CI/CD deployment support.
                        </p>
                    </div>
                </div>

                <div className="mt-10 flex flex-col gap-3 border-t border-slate-800 pt-6 text-sm text-slate-500 sm:flex-row sm:items-center sm:justify-between">
                    <p>
                        © {currentYear} ResumeApp. All rights reserved.
                    </p>

                    <p>
                        Built by{" "}
                        <span className="font-semibold text-slate-300">
                            Future Ibeche
                        </span>
                    </p>
                </div>
            </div>
        </footer>
    );
}

export default AppFooter;