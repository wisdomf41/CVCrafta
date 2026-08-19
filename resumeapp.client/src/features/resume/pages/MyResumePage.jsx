import { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import axiosClient from "../../../core/api/axiosClient";

const createEmptyForm = (userName = "", userEmail = "") => ({
    name: userName,
    title: "",
    email: userEmail,
    url: "",
    stack: "",
    country: "",
    summary: "",
});

const mapResumeToForm = (resume) => ({
    name: resume?.name || "",
    title: resume?.title || "",
    email: resume?.email || "",
    url: resume?.url || "",
    stack: resume?.stack || "",
    country: resume?.country || "",
    summary: resume?.summary || "",
});

const getFirstResume = (data) => {
    if (Array.isArray(data)) {
        return data[0] || null;
    }

    return data || null;
};

const getApiErrorMessage = (error, fallbackMessage) => {
    const responseData = error.response?.data;

    if (typeof responseData === "string") {
        return responseData;
    }

    if (responseData?.errors) {
        return Object.values(responseData.errors).flat().join(" ");
    }

    return (
        responseData?.message ||
        responseData?.title ||
        error.message ||
        fallbackMessage
    );
};

function MyResumePage() {
    const navigate = useNavigate();

    const userName = localStorage.getItem("user_name") || "";
    const userEmail = localStorage.getItem("user_email") || "";

    const [resume, setResume] = useState(null);
    const [editForm, setEditForm] = useState(() =>
        createEmptyForm(userName, userEmail)
    );

    const [isEditing, setIsEditing] = useState(false);
    const [isLoading, setIsLoading] = useState(true);
    const [isSaving, setIsSaving] = useState(false);
    const [error, setError] = useState("");
    const [successMessage, setSuccessMessage] = useState("");

    // Updated: supports first-resume creation and accessible required-field labels.
    useEffect(() => {
        const fetchMyResume = async () => {
            try {
                setIsLoading(true);
                setError("");

                const response = await axiosClient.get("/Resume/my");
                const fetchedResume = getFirstResume(response.data);

                if (!fetchedResume) {
                    setResume(null);
                    setEditForm(createEmptyForm(userName, userEmail));
                    return;
                }

                setResume(fetchedResume);
                setEditForm(mapResumeToForm(fetchedResume));
            } catch (requestError) {
                console.error("Failed to fetch my resume:", requestError);

                if (requestError.response?.status === 404) {
                    setResume(null);
                    setEditForm(createEmptyForm(userName, userEmail));
                    setError("");
                    return;
                }

                setError(
                    getApiErrorMessage(
                        requestError,
                        "Unable to load your resume."
                    )
                );
            } finally {
                setIsLoading(false);
            }
        };

        fetchMyResume();
    }, [userEmail, userName]);

    const handleFormChange = (event) => {
        const { name, value } = event.target;

        setEditForm((previous) => ({
            ...previous,
            [name]: value,
        }));
    };

    const handleCreateResume = async (event) => {
        event.preventDefault();

        try {
            setIsSaving(true);
            setError("");
            setSuccessMessage("");

            const response = await axiosClient.post("/Resume", editForm);
            const createdResume = getFirstResume(response.data);

            if (!createdResume) {
                throw new Error("The API did not return the created resume.");
            }

            setResume(createdResume);
            setEditForm(mapResumeToForm(createdResume));
            setSuccessMessage("Your resume was created successfully.");
            setIsEditing(false);
        } catch (requestError) {
            console.error("Failed to create resume:", requestError);

            setError(
                getApiErrorMessage(
                    requestError,
                    "Unable to create your resume."
                )
            );
        } finally {
            setIsSaving(false);
        }
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

            const updatedResume = {
                ...resume,
                ...editForm,
                updatedAt: new Date().toISOString(),
            };

            setResume(updatedResume);
            setEditForm(mapResumeToForm(updatedResume));
            setSuccessMessage("Resume updated successfully.");
            setIsEditing(false);
        } catch (requestError) {
            console.error("Failed to update resume:", requestError);

            setError(
                getApiErrorMessage(
                    requestError,
                    "Unable to update your resume."
                )
            );
        } finally {
            setIsSaving(false);
        }
    };

    const handleCancelEdit = () => {
        if (!resume) {
            return;
        }

        setEditForm(mapResumeToForm(resume));
        setError("");
        setSuccessMessage("");
        setIsEditing(false);
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
                                {userName
                                    ? `Welcome, ${userName}`
                                    : "Manage your resume"}
                            </h1>

                            <p className="mt-2 max-w-2xl text-slate-600">
                                Create, update, and manage your professional
                                resume from this private workspace.
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
                                type="button"
                                onClick={() => navigate("/dashboard")}
                                className="rounded-xl border border-slate-300 px-5 py-3 font-semibold text-slate-700 transition hover:bg-slate-50"
                            >
                                Back to Dashboard
                            </button>

                            {resume && (
                                <Link
                                    to="/my-resume/preview"
                                    className="rounded-xl bg-blue-600 px-5 py-3 text-center font-semibold text-white transition hover:bg-blue-700"
                                >
                                    Preview Resume
                                </Link>
                            )}
                        </div>
                    </div>
                </div>

                {isLoading && (
                    <div className="mt-8 rounded-2xl bg-white p-8 shadow-sm">
                        <p className="text-slate-600">
                            Loading your resume...
                        </p>
                    </div>
                )}

                {!isLoading && error && (
                    <div
                        role="alert"
                        className="mt-8 rounded-2xl border border-red-200 bg-red-50 p-5 text-red-700"
                    >
                        {error}
                    </div>
                )}

                {!isLoading && successMessage && (
                    <div
                        role="status"
                        className="mt-8 rounded-2xl border border-green-200 bg-green-50 p-5 text-green-700"
                    >
                        {successMessage}
                    </div>
                )}

                {!isLoading && !resume && !error && (
                    <ResumeForm
                        form={editForm}
                        onChange={handleFormChange}
                        onSubmit={handleCreateResume}
                        isSaving={isSaving}
                        mode="create"
                    />
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
                                    {resume.email && (
                                        <span>📧 {resume.email}</span>
                                    )}

                                    {resume.url && (
                                        <span>🔗 {resume.url}</span>
                                    )}

                                    {resume.country && (
                                        <span>📍 {resume.country}</span>
                                    )}
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
                                <h3 className="font-bold text-slate-900">
                                    Summary
                                </h3>

                                <p className="mt-2 leading-7 text-slate-700">
                                    {resume.summary}
                                </p>
                            </div>
                        )}
                    </div>
                )}

                {!isLoading && resume && isEditing && (
                    <ResumeForm
                        form={editForm}
                        onChange={handleFormChange}
                        onSubmit={handleSaveBasicInfo}
                        onCancel={handleCancelEdit}
                        isSaving={isSaving}
                        mode="edit"
                    />
                )}

                {!isLoading && resume && (
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
                )}
            </section>
        </main>
    );
}

function ResumeForm({
    form,
    onChange,
    onSubmit,
    onCancel,
    isSaving,
    mode,
}) {
    const isCreating = mode === "create";

    return (
        <form
            onSubmit={onSubmit}
            className="mt-8 rounded-2xl bg-white p-8 shadow-sm"
        >
            <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
                <div>
                    <p className="text-sm font-semibold uppercase tracking-wide text-blue-600">
                        {isCreating
                            ? "Create Your First Resume"
                            : "Edit Basic Information"}
                    </p>

                    <h2 className="mt-2 text-2xl font-bold text-slate-900">
                        {isCreating
                            ? "Enter your resume details"
                            : "Update your resume details"}
                    </h2>

                    {isCreating && (
                        <p className="mt-2 text-slate-600">
                            You do not have a resume yet. Complete the form
                            below to create one.
                        </p>
                    )}
                </div>

                <div className="flex gap-3">
                    {!isCreating && (
                        <button
                            type="button"
                            onClick={onCancel}
                            disabled={isSaving}
                            className="rounded-xl border border-slate-300 px-5 py-3 font-semibold text-slate-700 transition hover:bg-slate-50 disabled:cursor-not-allowed disabled:opacity-60"
                        >
                            Cancel
                        </button>
                    )}

                    <button
                        type="submit"
                        disabled={isSaving}
                        className="rounded-xl bg-blue-600 px-5 py-3 font-semibold text-white transition hover:bg-blue-700 disabled:cursor-not-allowed disabled:bg-blue-300"
                    >
                        {isSaving
                            ? isCreating
                                ? "Creating..."
                                : "Saving..."
                            : isCreating
                                ? "Create Resume"
                                : "Save Changes"}
                    </button>
                </div>
            </div>

            <div className="mt-6 grid gap-5 md:grid-cols-2">
                <FormInput
                    label="Name"
                    name="name"
                    value={form.name}
                    onChange={onChange}
                    placeholder="Your full name"
                    required
                />

                <FormInput
                    label="Title"
                    name="title"
                    value={form.title}
                    onChange={onChange}
                    placeholder="Full-Stack .NET Developer"
                    required
                />

                <FormInput
                    label="Email"
                    name="email"
                    type="email"
                    value={form.email}
                    onChange={onChange}
                    placeholder="you@example.com"
                />

                <FormInput
                    label="URL"
                    name="url"
                    value={form.url}
                    onChange={onChange}
                    placeholder="Portfolio, LinkedIn, or public resume URL"
                    required
                />

                <FormInput
                    label="Stack"
                    name="stack"
                    value={form.stack}
                    onChange={onChange}
                    placeholder="C#, ASP.NET Core, React, SQL Server"
                    required
                />

                <FormInput
                    label="Country"
                    name="country"
                    value={form.country}
                    onChange={onChange}
                    placeholder="Nigeria"
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
                    value={form.summary}
                    onChange={onChange}
                    rows="5"
                    className="w-full rounded-xl border border-slate-300 px-4 py-3 outline-none transition focus:border-blue-500 focus:ring-2 focus:ring-blue-100"
                    placeholder="Write your professional summary"
                />
            </div>
        </form>
    );
}

function FormInput({
    label,
    name,
    value,
    onChange,
    placeholder,
    type = "text",
    required = false,
}) {
    return (
        <div>
            <label
                htmlFor={name}
                className="mb-2 block text-sm font-medium text-slate-700"
            >
                {label}
                {required && (
                    <span aria-hidden="true" className="text-red-600">
                        {" "}*
                    </span>
                )}
            </label>

            <input
                id={name}
                name={name}
                type={type}
                value={value}
                onChange={onChange}
                required={required}
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
                <h2 className="text-lg font-bold text-slate-900">
                    {title}
                </h2>

                <span className="rounded-full bg-slate-100 px-3 py-1 text-xs font-semibold text-slate-600">
                    {status}
                </span>
            </div>

            <p className="mt-3 text-sm leading-6 text-slate-600">
                {description}
            </p>

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