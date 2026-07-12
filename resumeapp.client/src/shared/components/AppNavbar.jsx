import { useState } from "react";
import { Link, useLocation, useNavigate } from "react-router-dom";

function AppNavbar() {
    const [isMenuOpen, setIsMenuOpen] = useState(false);

    const navigate = useNavigate();
    const location = useLocation();

    const token = localStorage.getItem("resume_app_token");
    const isLoggedIn = Boolean(token);

    const handleLogout = () => {
        localStorage.removeItem("resume_app_token");
        localStorage.removeItem("user_email");
        localStorage.removeItem("user_name");

        setIsMenuOpen(false);
        navigate("/login");
    };

    const handleLinkClick = () => {
        setIsMenuOpen(false);
    };

    const isActive = (path) => location.pathname === path;

    const linkClass = (path) =>
        `rounded-xl px-4 py-2 text-sm font-semibold transition ${isActive(path)
            ? "bg-slate-950 text-white shadow-sm"
            : "text-slate-600 hover:bg-slate-100 hover:text-slate-950"
        }`;

    const mobileLinkClass = (path) =>
        `block w-full rounded-xl px-4 py-3 text-left text-sm font-semibold transition ${isActive(path)
            ? "bg-slate-950 text-white"
            : "text-slate-700 hover:bg-slate-100"
        }`;

    return (
        <nav className="sticky top-0 z-50 border-b border-slate-200/80 bg-white/90 px-4 py-3 shadow-sm backdrop-blur">
            <div className="mx-auto max-w-6xl">
                <div className="flex items-center justify-between gap-4">
                    <Link
                        to="/"
                        onClick={handleLinkClick}
                        className="flex items-center gap-2"
                    >
                        <div className="flex h-10 w-10 items-center justify-center rounded-2xl bg-slate-950 text-lg font-extrabold text-white shadow-sm">
                            R
                        </div>

                        <div>
                            <p className="text-lg font-extrabold tracking-tight text-slate-950">
                                ResumeApp
                            </p>

                            <p className="-mt-1 hidden text-xs font-medium text-slate-500 sm:block">
                                Build. Manage. Share.
                            </p>
                        </div>
                    </Link>

                    <div className="hidden items-center gap-2 md:flex">
                        <Link to="/" className={linkClass("/")}>
                            Home
                        </Link>

                        <Link to="/resume" className={linkClass("/resume")}>
                            Public Resume
                        </Link>

                        {isLoggedIn ? (
                            <>
                                <Link to="/dashboard" className={linkClass("/dashboard")}>
                                    Dashboard
                                </Link>

                                <Link to="/my-resume" className={linkClass("/my-resume")}>
                                    My Resume
                                </Link>

                                <Link
                                    to="/my-resume/preview"
                                    className={linkClass("/my-resume/preview")}
                                >
                                    Preview
                                </Link>

                                <button
                                    type="button"
                                    onClick={handleLogout}
                                    className="rounded-xl bg-red-600 px-4 py-2 text-sm font-semibold text-white transition hover:bg-red-700"
                                >
                                    Logout
                                </button>
                            </>
                        ) : (
                            <>
                                <Link to="/login" className={linkClass("/login")}>
                                    Login
                                </Link>

                                <Link
                                    to="/register"
                                    className="rounded-xl bg-emerald-600 px-4 py-2 text-sm font-semibold text-white shadow-sm transition hover:bg-emerald-700"
                                >
                                    Register
                                </Link>
                            </>
                        )}
                    </div>

                    <button
                        type="button"
                        onClick={() => setIsMenuOpen((current) => !current)}
                        aria-label="Toggle navigation menu"
                        aria-expanded={isMenuOpen}
                        className="flex h-11 w-11 items-center justify-center rounded-xl border border-slate-300 bg-white text-slate-800 transition hover:bg-slate-100 md:hidden"
                    >
                        {isMenuOpen ? (
                            <span className="text-2xl leading-none">×</span>
                        ) : (
                            <span className="text-2xl leading-none">☰</span>
                        )}
                    </button>
                </div>

                {isMenuOpen && (
                    <div className="mt-4 space-y-2 rounded-2xl border border-slate-200 bg-white p-3 shadow-lg md:hidden">
                        <Link
                            to="/"
                            onClick={handleLinkClick}
                            className={mobileLinkClass("/")}
                        >
                            Home
                        </Link>

                        <Link
                            to="/resume"
                            onClick={handleLinkClick}
                            className={mobileLinkClass("/resume")}
                        >
                            Public Resume
                        </Link>

                        {isLoggedIn ? (
                            <>
                                <Link
                                    to="/dashboard"
                                    onClick={handleLinkClick}
                                    className={mobileLinkClass("/dashboard")}
                                >
                                    Dashboard
                                </Link>

                                <Link
                                    to="/my-resume"
                                    onClick={handleLinkClick}
                                    className={mobileLinkClass("/my-resume")}
                                >
                                    My Resume
                                </Link>

                                <Link
                                    to="/my-resume/preview"
                                    onClick={handleLinkClick}
                                    className={mobileLinkClass("/my-resume/preview")}
                                >
                                    Preview Resume
                                </Link>

                                <button
                                    type="button"
                                    onClick={handleLogout}
                                    className="w-full rounded-xl bg-red-600 px-4 py-3 text-left text-sm font-semibold text-white transition hover:bg-red-700"
                                >
                                    Logout
                                </button>
                            </>
                        ) : (
                            <>
                                <Link
                                    to="/login"
                                    onClick={handleLinkClick}
                                    className={mobileLinkClass("/login")}
                                >
                                    Login
                                </Link>

                                <Link
                                    to="/register"
                                    onClick={handleLinkClick}
                                    className="block w-full rounded-xl bg-emerald-600 px-4 py-3 text-left text-sm font-semibold text-white transition hover:bg-emerald-700"
                                >
                                    Create Account
                                </Link>
                            </>
                        )}
                    </div>
                )}
            </div>
        </nav>
    );
}

export default AppNavbar;