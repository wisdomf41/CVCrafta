import { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import axiosClient from "../../../core/api/axiosClient";

function MyResumePage() {
    const navigate = useNavigate();

    const [resume, setResume] = useState(null);
    const [editForm, setEditForm] = useState({
        name: "",
        title: "",
        email: "",
        url: "",
        stack: "",
        country: "",
        summary: "",
    });

    const [isEditing, setIsEditing] = useState(false);
    const [isLoading, setIsLoading] = useState(true);
    const [isSaving, setIsSaving] = useState(false);
    const [error, setError] = useState("");
    const [successMessage, setSuccessMessage] = useState("");

    const userName = localStorage.getItem("user_name");
    const userEmail = localStorage.getItem("user_email");

    useEffect(() => {
        const fetchMyResume = async () => {
            try {
                setIsLoading(true);
                setError("");

                const response = await axiosClient.get("/Resume/my");

                setResume(response.data);

                setEditForm({
                    name: response.data.name || "",
                    title: response.data.title || "",
                    email: response.data.email || "",
                    url: response.data.url || "",
                    stack: response.data.stack || "",
                    country: response.data.country || "",
                    summary: response.data.summary || "",
                });
            } catch (error) {
                console.error("Failed to fetch my resume:", error);

                const message =
                    error.response?.data?.message ||
                    error.response?.data?.title ||
                    error.response?.data ||
                    error.message ||
                    "Unable to load your resume.";

                setError(String(message));
            } finally {
                setIsLoading(false);
            }
        };

        fetchMyResume();
    }, []);

    const handleEditChange = (event) => {
        const { name, value } = event.target;

        setEditForm((previous) => ({
            ...previous,
            [name]: value,
        }));
    };

    const handleCancelEdit = () => {
        if (!resume) return;

        setEditForm({
            name: resume.name || "",
            title: resume.title || "",
            email: resume.email || "",
            url: resume.url || "",
            stack: resume.stack || "",
            country: resume.country || "",
            summary: resume.summary || "",
        });

        setError("");
        setSuccessMessage("");
        setIsEditing(false);
    };

    const handleSaveBasicInfo = async (event) => {
        event.preventDefault();

        if (!resume?.id) {
            setError("Resume ID not found.");
            return;
        }

        try {
            setIsSaving(true);
            setError("");
            setSuccessMessage("");

            await axiosClient.put(`/Resume/${resume.id}`, editForm);

            setResume((previous) => ({
                ...previous,
                ...editForm,
                updatedAt: new Date().toISOString(),
            }));

            setSuccessMessage("Resume updated successfully.");
            setIsEditing(false);
        } catch (error) {
            console.error("Failed to update resume:", error);

            const message =
                error.response?.data?.message ||
                error.response?.data?.title ||
                error.response?.data ||
                error.message ||
                "Unable to update resume.";

            setError(String(message));
        } finally {
            setIsSaving(false);
        }
    };

    return (
        <main className="min-h-screen bg-slate-100 px-6 py-10">
            <section className="mx-auto max-w-6xl">
                <div className="rounded-2xl bg-white p-8 shadow-sm">
                    <div className="flex flex-col gap-5 md:flex-row md:items-center md:justify-between">
                        <div>
                            <p className="text-sm font-semibold uppercase tracking-wide text-blue-600">
                                My Resume
                            </p>

                            <h1 className="mt-2 text-3xl font-bold text-slate-900">
                                {userName ? `Welcome, ${userName}` : "Manage your resume"}
                            </h1>

                            <p className="mt-2 max-w-2xl text-slate-600">
                                This is your private resume workspace. From here, you will
                                create, update, and manage your resume sections.
                            </p>

                            {userEmail && (
                                <p className="mt-3 text-sm text-slate-500">
                                    Signed in as{" "}
                                    <span className="font-semibold text-slate-700">
                                        {userEmail}
                                    </span>
                                </p>
                            )}
                        </div>

                        <div className="flex flex-col gap-3 sm:flex-row">
                            <button
                                onClick={() => navigate("/dashboard")}
                                className="rounded-xl border border-slate-300 px-5 py-3 font-semibold text-slate-700 transition hover:bg-slate-50"
                            >
                                Back to Dashboard
                            </button>

                            <Link
                                to="/my-resume/preview"
                                className="rounded-xl bg-blue-600 px-5 py-3 text-center font-semibold text-white transition hover:bg-blue-700"
                            >
                                Preview Updated Resume
                            </Link>
                        </div>
                    </div>
                </div>

                {isLoading && (
                    <div className="mt-8 rounded-2xl bg-white p-8 shadow-sm">
                        <p className="text-slate-600">Loading your resume...</p>
                    </div>
                )}

                {!isLoading && error && (
                    <div className="mt-8 rounded-2xl border border-red-200 bg-red-50 p-5 text-red-700">
                        {error}
                    </div>
                )}

                {!isLoading && successMessage && (
                    <div className="mt-8 rounded-2xl border border-green-200 bg-green-50 p-5 text-green-700">
                        {successMessage}
                    </div>
                )}

                {!isLoading && resume && !isEditing && (
                    <div className="mt-8 rounded-2xl bg-white p-8 shadow-sm">
                        <div className="flex flex-col gap-5 md:flex-row md:items-start md:justify-between">
                            <div>
                                <p className="text-sm font-semibold uppercase tracking-wide text-emerald-600">
                                    Current Resume
                                </p>

                                <h2 className="mt-2 text-3xl font-bold text-slate-900">
                                    {resume.name}
                                </h2>

                                <p className="mt-2 text-xl font-semibold text-blue-600">
                                    {resume.title}
                                </p>

                                <div className="mt-4 flex flex-wrap gap-3 text-sm text-slate-600">
                                    {resume.email && <span>📧 {resume.email}</span>}
                                    {resume.url && <span>🔗 {resume.url}</span>}
                                    {resume.country && <span>📍 {resume.country}</span>}
                                </div>

                                {resume.stack && (
                                    <p className="mt-4 text-sm font-medium text-slate-700">
                                        {resume.stack}
                                    </p>
                                )}
                            </div>

                            <button
                                type="button"
                                onClick={() => {
                                    setError("");
                                    setSuccessMessage("");
                                    setIsEditing(true);
                                }}
                                className="rounded-xl bg-slate-900 px-5 py-3 font-semibold text-white transition hover:bg-slate-800"
                            >
                                Edit Basic Info
                            </button>
                        </div>

                        {resume.summary && (
                            <div className="mt-6 rounded-2xl bg-slate-50 p-5">
                                <h3 className="font-bold text-slate-900">Summary</h3>
                                <p className="mt-2 leading-7 text-slate-700">
                                    {resume.summary}
                                </p>
                            </div>
                        )}
                    </div>
                )}

                {!isLoading && resume && isEditing && (
                    <form
                        onSubmit={handleSaveBasicInfo}
                        className="mt-8 rounded-2xl bg-white p-8 shadow-sm"
                    >
                        <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
                            <div>
                                <p className="text-sm font-semibold uppercase tracking-wide text-blue-600">
                                    Edit Basic Information
                                </p>

                                <h2 className="mt-2 text-2xl font-bold text-slate-900">
                                    Update your resume details
                                </h2>
                            </div>

                            <div className="flex gap-3">
                                <button
                                    type="button"
                                    onClick={handleCancelEdit}
                                    className="rounded-xl border border-slate-300 px-5 py-3 font-semibold text-slate-700 transition hover:bg-slate-50"
                                >
                                    Cancel
                                </button>

                                <button
                                    type="submit"
                                    disabled={isSaving}
                                    className="rounded-xl bg-blue-600 px-5 py-3 font-semibold text-white transition hover:bg-blue-700 disabled:cursor-not-allowed disabled:bg-blue-300"
                                >
                                    {isSaving ? "Saving..." : "Save Changes"}
                                </button>
                            </div>
                        </div>

                        <div className="mt-6 grid gap-5 md:grid-cols-2">
                            <FormInput
                                label="Name"
                                name="name"
                                value={editForm.name}
                                onChange={handleEditChange}
                                placeholder="Your full name"
                            />

                            <FormInput
                                label="Title"
                                name="title"
                                value={editForm.title}
                                onChange={handleEditChange}
                                placeholder="C# /.NET Developer"
                            />

                            <FormInput
                                label="Email"
                                name="email"
                                type="email"
                                value={editForm.email}
                                onChange={handleEditChange}
                                placeholder="you@example.com"
                            />

                            <FormInput
                                label="URL"
                                name="url"
                                value={editForm.url}
                                onChange={handleEditChange}
                                placeholder="LinkedIn, portfolio, or website"
                            />

                            <FormInput
                                label="Stack"
                                name="stack"
                                value={editForm.stack}
                                onChange={handleEditChange}
                                placeholder="C#, ASP.NET Core, React, SQL Server"
                            />

                            <FormInput
                                label="Country"
                                name="country"
                                value={editForm.country}
                                onChange={handleEditChange}
                                placeholder="United Kingdom"
                            />
                        </div>

                        <div className="mt-5">
                            <label
                                htmlFor="summary"
                                className="mb-2 block text-sm font-medium text-slate-700"
                            >
                                Summary
                            </label>

                            <textarea
                                id="summary"
                                name="summary"
                                value={editForm.summary}
                                onChange={handleEditChange}
                                rows="5"
                                className="w-full rounded-xl border border-slate-300 px-4 py-3 outline-none transition focus:border-blue-500 focus:ring-2 focus:ring-blue-100"
                                placeholder="Write your professional summary"
                            />
                        </div>
                    </form>
                )}

                <div className="mt-8 grid gap-5 md:grid-cols-2 lg:grid-cols-3">
                    <ResumeSectionCard
                        title="Experience"
                        description="Add, update, and organize your professional experience."
                        status="Coming soon"
                    />

                    <ResumeSectionCard
                        title="Skills"
                        description="Manage technical skills by category and display order."
                        status="Coming soon"
                    />

                    <ResumeSectionCard
                        title="Projects"
                        description="Showcase your selected technical projects and links."
                        status="Coming soon"
                    />

                    <ResumeSectionCard
                        title="Certifications"
                        description="Manage certifications, issuers, years, and verification links."
                        status="Coming soon"
                    />

                    <ResumeSectionCard
                        title="Education"
                        description="Manage education, qualifications, grades, and graduation year."
                        status="Coming soon"
                    />

                    <ResumeSectionCard
                        title="Referees"
                        description="Add referees or show available on request."
                        status="Coming soon"
                    />
                </div>
            </section>
        </main>
    );
}

function FormInput({
    label,
    name,
    value,
    onChange,
    placeholder,
    type = "text",
}) {
    return (
        <div>
            <label
                htmlFor={name}
                className="mb-2 block text-sm font-medium text-slate-700"
            >
                {label}
            </label>

            <input
                id={name}
                name={name}
                type={type}
                value={value}
                onChange={onChange}
                className="w-full rounded-xl border border-slate-300 px-4 py-3 outline-none transition focus:border-blue-500 focus:ring-2 focus:ring-blue-100"
                placeholder={placeholder}
            />
        </div>
    );
}

function ResumeSectionCard({ title, description, status }) {
    return (
        <article className="rounded-2xl bg-white p-6 shadow-sm transition hover:-translate-y-1 hover:shadow-md">
            <div className="flex items-start justify-between gap-4">
                <h2 className="text-lg font-bold text-slate-900">{title}</h2>

                <span className="rounded-full bg-slate-100 px-3 py-1 text-xs font-semibold text-slate-600">
                    {status}
                </span>
            </div>

            <p className="mt-3 text-sm leading-6 text-slate-600">{description}</p>

            <button
                type="button"
                disabled
                className="mt-5 rounded-xl bg-slate-200 px-4 py-2 text-sm font-semibold text-slate-500"
            >
                Manage
            </button>
        </article>
    );
}

export default MyResumePage;