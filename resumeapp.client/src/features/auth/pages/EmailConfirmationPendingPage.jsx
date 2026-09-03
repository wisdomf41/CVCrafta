import { useRef, useState } from "react";
import { Link, useLocation } from "react-router-dom";
import axiosClient from "../../../core/api/axiosClient";

// Presents safe confirmation guidance and guards resend requests from duplication.
function EmailConfirmationPendingPage() {
    const location = useLocation();
    const email =
        typeof location.state?.email === "string"
            ? location.state.email.trim()
            : "";
    const resendInFlight = useRef(false);
    const [isResending, setIsResending] = useState(false);
    const [resendSuccess, setResendSuccess] = useState("");
    const [resendError, setResendError] = useState("");

    const handleResend = async () => {
        if (!email || resendInFlight.current) {
            return;
        }

        resendInFlight.current = true;
        setIsResending(true);
        setResendSuccess("");
        setResendError("");

        try {
            await axiosClient.post("/auth/resend-email-confirmation", {
                email,
            });

            setResendSuccess(
                "Confirmation email sent. Check your inbox and spam/junk folder."
            );
        } catch (error) {
            const message =
                error.response?.data?.message ||
                error.response?.data?.title ||
                error.response?.data ||
                error.message ||
                "We could not resend the confirmation email. Please try again.";

            setResendError(String(message));
        } finally {
            resendInFlight.current = false;
            setIsResending(false);
        }
    };

    return (
        <main className="flex min-h-screen items-center justify-center bg-gradient-to-br from-slate-50 via-blue-50 to-emerald-50 px-4 py-12 sm:px-6">
            <section className="w-full max-w-xl rounded-3xl border border-slate-200 bg-white p-7 text-center shadow-2xl sm:p-10">
                <div
                    aria-hidden="true"
                    className="mx-auto flex h-16 w-16 items-center justify-center rounded-full bg-emerald-100 text-3xl text-emerald-700"
                >
                    ✓
                </div>

                {email ? (
                    <>
                        <p className="mt-6 text-sm font-semibold uppercase tracking-[0.2em] text-emerald-700">
                            Registration complete
                        </p>
                        <h1 className="mt-3 text-3xl font-extrabold tracking-tight text-slate-950 sm:text-4xl">
                            Confirm your email
                        </h1>
                        <p className="mt-5 text-lg font-semibold leading-8 text-slate-800">
                            Account created successfully. Check your email to confirm your
                            account before signing in.
                        </p>
                        <p className="mt-4 text-slate-600">
                            We sent a confirmation link to{" "}
                            <strong className="break-all font-bold text-slate-900">
                                {email}
                            </strong>
                            .
                        </p>
                        <p className="mt-3 leading-7 text-slate-600">
                            Check your inbox and spam/junk folder, then follow the link in
                            the email.
                        </p>

                        {resendSuccess && (
                            <div
                                role="status"
                                className="mt-6 rounded-2xl border border-emerald-200 bg-emerald-50 p-4 text-sm text-emerald-800"
                            >
                                {resendSuccess}
                            </div>
                        )}

                        {resendError && (
                            <div
                                role="alert"
                                className="mt-6 rounded-2xl border border-red-200 bg-red-50 p-4 text-sm text-red-700"
                            >
                                {resendError}
                            </div>
                        )}

                        <div className="mt-7 flex flex-col gap-3 sm:flex-row sm:justify-center">
                            <Link
                                to="/login"
                                className="rounded-xl bg-blue-600 px-6 py-3 font-bold text-white shadow-lg shadow-blue-200 transition hover:bg-blue-700"
                            >
                                Return to login
                            </Link>
                            <button
                                type="button"
                                onClick={handleResend}
                                disabled={isResending}
                                className="rounded-xl border border-slate-300 bg-white px-6 py-3 font-bold text-slate-800 transition hover:bg-slate-50 disabled:cursor-not-allowed disabled:bg-slate-100 disabled:text-slate-400"
                            >
                                {isResending
                                    ? "Resending confirmation email..."
                                    : "Resend confirmation email"}
                            </button>
                        </div>
                    </>
                ) : (
                    <>
                        <p className="mt-6 text-sm font-semibold uppercase tracking-[0.2em] text-blue-700">
                            Email confirmation
                        </p>
                        <h1 className="mt-3 text-3xl font-extrabold tracking-tight text-slate-950 sm:text-4xl">
                            Check your email
                        </h1>
                        <p className="mt-5 leading-7 text-slate-600">
                            If you just created an account, check your inbox and spam/junk
                            folder for a confirmation link before signing in.
                        </p>
                        <p className="mt-3 leading-7 text-slate-600">
                            We do not have an email address for this page, so return to
                            registration if you still need to create an account.
                        </p>

                        <div className="mt-7 flex flex-col gap-3 sm:flex-row sm:justify-center">
                            <Link
                                to="/register"
                                className="rounded-xl bg-blue-600 px-6 py-3 font-bold text-white shadow-lg shadow-blue-200 transition hover:bg-blue-700"
                            >
                                Register
                            </Link>
                            <Link
                                to="/login"
                                className="rounded-xl border border-slate-300 bg-white px-6 py-3 font-bold text-slate-800 transition hover:bg-slate-50"
                            >
                                Return to login
                            </Link>
                        </div>
                    </>
                )}
            </section>
        </main>
    );
}

export default EmailConfirmationPendingPage;
