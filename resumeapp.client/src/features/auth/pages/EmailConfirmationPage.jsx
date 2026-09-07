import { useEffect, useLayoutEffect, useRef, useState } from "react";
import { Link, useLocation, useNavigate } from "react-router-dom";
import axiosClient from "../../../core/api/axiosClient";
import storeAuthentication from "../utils/authStorage";

const confirmationFailureMessage =
    "We could not confirm this email link. It may be invalid, expired, or already used.";

// Processes confirmation once after removing sensitive query data from history.
function EmailConfirmationPage() {
    const location = useLocation();
    const navigate = useNavigate();
    const confirmation = useRef();
    const confirmationStarted = useRef(false);
    const resendStarted = useRef(false);
    const [status, setStatus] = useState("processing");
    const [email, setEmail] = useState("");
    const [isResending, setIsResending] = useState(false);
    const [resendMessage, setResendMessage] = useState("");

    if (!confirmation.current) {
        const encodedConfirmation = location.hash
            ? location.hash.slice(1)
            : location.search;
        const parameters = new URLSearchParams(encodedConfirmation);

        confirmation.current = {
            userId: parameters.get("userId")?.trim() ?? "",
            token: parameters.get("token")?.trim() ?? "",
        };
    }

    useLayoutEffect(() => {
        if (location.search || location.hash) {
            window.history.replaceState(
                window.history.state,
                document.title,
                location.pathname
            );
        }
    }, [location.hash, location.pathname, location.search]);

    useEffect(() => {
        if (confirmationStarted.current) {
            return;
        }

        confirmationStarted.current = true;

        const confirmEmail = async () => {
            const { userId, token } = confirmation.current;

            if (!userId || !token) {
                setStatus("failed");
                return;
            }

            try {
                const response = await axiosClient.post(
                    "/auth/confirm-email",
                    { userId, token }
                );

                storeAuthentication(response.data);
                navigate("/dashboard", { replace: true });
            } catch {
                setStatus("failed");
            }
        };

        confirmEmail();
    }, [navigate]);

    const handleResend = async (event) => {
        event.preventDefault();

        if (!email.trim() || resendStarted.current) {
            return;
        }

        resendStarted.current = true;
        setIsResending(true);
        setResendMessage("");

        try {
            await axiosClient.post("/auth/resend-email-confirmation", {
                email: email.trim(),
            });

            setResendMessage(
                "If an unverified account exists, a new confirmation email has been requested."
            );
        } catch {
            setResendMessage(
                "We could not request another confirmation email. Please try again."
            );
        } finally {
            resendStarted.current = false;
            setIsResending(false);
        }
    };

    if (status === "processing") {
        return (
            <main className="flex min-h-screen items-center justify-center bg-gradient-to-br from-slate-50 via-blue-50 to-emerald-50 px-4 py-12 sm:px-6">
                <section
                    aria-live="polite"
                    className="w-full max-w-xl rounded-3xl border border-slate-200 bg-white p-8 text-center shadow-2xl sm:p-10"
                >
                    <div
                        aria-hidden="true"
                        className="mx-auto h-12 w-12 animate-spin rounded-full border-4 border-blue-100 border-t-blue-600"
                    />
                    <h1 className="mt-6 text-3xl font-extrabold tracking-tight text-slate-950">
                        Confirming your email
                    </h1>
                    <p className="mt-4 text-slate-600" role="status">
                        Please wait while we securely confirm your account.
                    </p>
                </section>
            </main>
        );
    }

    return (
        <main className="flex min-h-screen items-center justify-center bg-gradient-to-br from-slate-50 via-blue-50 to-emerald-50 px-4 py-12 sm:px-6">
            <section className="w-full max-w-xl rounded-3xl border border-slate-200 bg-white p-7 shadow-2xl sm:p-10">
                <div className="text-center">
                    <div
                        aria-hidden="true"
                        className="mx-auto flex h-16 w-16 items-center justify-center rounded-full bg-amber-100 text-3xl text-amber-700"
                    >
                        !
                    </div>
                    <h1 className="mt-6 text-3xl font-extrabold tracking-tight text-slate-950">
                        Confirmation unsuccessful
                    </h1>
                    <p className="mt-4 leading-7 text-slate-600" role="alert">
                        {confirmationFailureMessage}
                    </p>
                </div>

                <form className="mt-7 space-y-4" onSubmit={handleResend}>
                    <div>
                        <label
                            className="mb-2 block text-sm font-semibold text-slate-700"
                            htmlFor="confirmation-email"
                        >
                            Email address
                        </label>
                        <input
                            autoComplete="email"
                            className="w-full rounded-xl border border-slate-300 bg-white px-4 py-3 text-slate-900 outline-none transition focus:border-blue-500 focus:ring-4 focus:ring-blue-100"
                            id="confirmation-email"
                            onChange={(event) => setEmail(event.target.value)}
                            required
                            type="email"
                            value={email}
                        />
                    </div>

                    <button
                        className="w-full rounded-xl bg-blue-600 px-5 py-3 font-bold text-white transition hover:bg-blue-700 disabled:cursor-not-allowed disabled:bg-blue-300"
                        disabled={isResending}
                        type="submit"
                    >
                        {isResending
                            ? "Requesting confirmation email..."
                            : "Request another confirmation email"}
                    </button>
                </form>

                {resendMessage && (
                    <p
                        className="mt-5 rounded-2xl border border-slate-200 bg-slate-50 p-4 text-sm text-slate-700"
                        role="status"
                    >
                        {resendMessage}
                    </p>
                )}

                <div className="mt-7 text-center">
                    <Link
                        className="font-bold text-blue-600 transition hover:text-blue-700 hover:underline"
                        to="/login"
                    >
                        Return to login
                    </Link>
                </div>
            </section>
        </main>
    );
}

export default EmailConfirmationPage;
