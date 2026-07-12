import React, { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import axiosClient from "../../../core/api/axiosClient";

function PublicResumePage({ mode = "public" }) {
    const [resume, setResume] = useState(null);
    const [error, setError] = useState(null);
    const [experience, setExperience] = useState([]);
    const [skills, setSkills] = useState([]);
    const [projects, setProjects] = useState([]);
    const [certifications, setCertifications] = useState([]);
    const [education, setEducation] = useState([]);
    const [interest, setInterest] = useState([]);
    const [referee, setReferee] = useState([]);

    useEffect(() => {
        const fetchData = async () => {
            try {
                if (mode === "preview") {
                    const resumeResponse = await axiosClient.get("/Resume/my");
                    setResume(resumeResponse.data);
                } else {
                    const resumeResponse = await axiosClient.get("/Resume");
    const resumeData = resumeResponse.data;

    const publicResume = Array.isArray(resumeData)
        ? resumeData.find(
            (item) =>
                item.name &&
                item.title &&
                item.email &&
                item.name.toLowerCase() !== "string" &&
                item.title.toLowerCase() !== "string" &&
                item.email.toLowerCase() !== "string"
        )
        : null;

    if (publicResume) {
        setResume(publicResume);
    } else {
        setError("No valid public resume found.");
        return;
    }
                
                }

                const [
                    experienceResponse,
                    skillsResponse,
                    projectsResponse,
                    certificationsResponse,
                    educationResponse,
                    interestResponse,
                    refereeResponse,
                ] = await Promise.all([
                    axiosClient.get("/Experience"),
                    axiosClient.get("/Skill"),
                    axiosClient.get("/Project"),
                    axiosClient.get("/Certification"),
                    axiosClient.get("/Education"),
                    axiosClient.get("/Interest"),
                    axiosClient.get("/Referee"),
                ]);

                setExperience(Array.isArray(experienceResponse.data) ? experienceResponse.data : []);
                setSkills(Array.isArray(skillsResponse.data) ? skillsResponse.data : []);
                setProjects(Array.isArray(projectsResponse.data) ? projectsResponse.data : []);
                setCertifications(Array.isArray(certificationsResponse.data) ? certificationsResponse.data : []);
                setEducation(Array.isArray(educationResponse.data) ? educationResponse.data : []);
                setInterest(Array.isArray(interestResponse.data) ? interestResponse.data : []);
                setReferee(Array.isArray(refereeResponse.data) ? refereeResponse.data : []);
            } catch (error) {
                console.error("Error fetching data:", error);

                const errorMessage =
                    error.response?.data?.message ||
                    error.response?.data?.title ||
                    error.response?.data ||
                    error.message ||
                    "Unable to load resume.";

                setError(String(errorMessage));
            }
        };

        fetchData();
    }, [mode]);

    if (error) {
        return (
            <div className="flex min-h-screen items-center justify-center bg-slate-100 px-4">
                <div className="w-full max-w-md rounded-2xl border border-slate-200 bg-white p-6 text-center shadow-lg">
                    <h3 className="text-xl font-bold text-red-600">
                        Error Loading Resume
                    </h3>

                    <p className="mt-3 text-slate-600">{error}</p>

                    <p className="mt-2 text-sm text-slate-500">
                        Make sure your database has resume data and backend is running.
                    </p>

                    <button
                        onClick={() => window.location.reload()}
                        className="mt-5 rounded-xl bg-slate-900 px-4 py-2 text-white transition hover:bg-slate-800"
                    >
                        Retry
                    </button>
                </div>
            </div>
        );
    }

    if (!resume) {
        return (
            <div className="flex min-h-screen items-center justify-center bg-slate-100">
                <p className="text-lg font-medium text-slate-700">
                    Loading resume...
                </p>
            </div>
        );
    }

    return (
        <div className="min-h-screen bg-slate-100 px-4 py-10">
            {mode === "preview" && (
                <div className="mx-auto mb-6 flex max-w-5xl flex-col gap-4 rounded-2xl bg-white p-4 shadow-sm sm:flex-row sm:items-center sm:justify-between">
                    <div>
                        <p className="text-sm font-semibold text-blue-600">
                            Resume Preview
                        </p>

                        <p className="text-sm text-slate-600">
                            This is how your updated resume currently looks.
                        </p>
                    </div>

                    <div className="flex flex-col gap-3 sm:flex-row">
                        <Link
                            to="/my-resume"
                            className="rounded-xl border border-slate-300 px-5 py-2.5 text-center font-semibold text-slate-700 transition hover:bg-slate-50"
                        >
                            Back to Edit
                        </Link>

                        <Link
                            to="/dashboard"
                            className="rounded-xl bg-slate-900 px-5 py-2.5 text-center font-semibold text-white transition hover:bg-slate-800"
                        >
                            Back to Dashboard
                        </Link>
                    </div>
                </div>
            )}

            <div className="mx-auto max-w-5xl overflow-hidden rounded-3xl border border-slate-200 bg-white shadow-2xl">
                <header className="bg-slate-950 px-8 py-10 text-white">
                    <h1 className="text-4xl font-bold tracking-tight md:text-5xl">
                        {resume.name}
                    </h1>

                    <p className="mt-3 text-2xl font-semibold text-emerald-400">
                        {resume.title}
                    </p>

                    <p className="mt-2 text-sm text-slate-300 md:text-base">
                        {resume.stack}
                    </p>

                    <div className="mt-6 flex flex-wrap gap-4 text-sm text-slate-200">
                        <span>📧 {resume.email}</span>
                        <span>🔗 {resume.url}</span>
                        <span>📍 {resume.country}</span>
                    </div>
                </header>

                <div className="space-y-8 px-8 py-8">
                    <section className="border-b border-slate-200 pb-6">
                        <h2 className="text-lg font-bold tracking-wide text-slate-900">
                            SUMMARY
                        </h2>

                        <p className="mt-3 leading-7 text-slate-700">
                            {resume.summary}
                        </p>
                    </section>

                    <section className="border-b border-slate-200 pb-6">
                        <h2 className="text-lg font-bold tracking-wide text-slate-900">
                            EXPERIENCE
                        </h2>

                        {experience.length > 0 ? (
                            <div className="mt-5 space-y-6">
                                {experience.map((item) => (
                                    <div
                                        key={item.id}
                                        className="rounded-2xl border border-slate-200 bg-slate-50 p-5"
                                    >
                                        <div className="flex flex-col gap-2 md:flex-row md:items-center md:justify-between">
                                            <h3 className="text-lg font-semibold text-slate-900">
                                                {item.role}
                                            </h3>

                                            <span className="text-sm font-medium text-emerald-700">
                                                {item.duration}
                                            </span>
                                        </div>

                                        <p className="mt-1 font-medium text-slate-600">
                                            {item.company}
                                        </p>

                                        <ul className="mt-4 list-disc space-y-2 pl-5 text-slate-700">
                                            {item.description?.split("|").map((point, index) => (
                                                <li key={index}>{point.trim()}</li>
                                            ))}
                                        </ul>
                                    </div>
                                ))}
                            </div>
                        ) : (
                            <p className="mt-3 text-slate-500">
                                No experience data found.
                            </p>
                        )}
                    </section>

                    <section className="border-b border-slate-200 pb-6">
                        <h2 className="text-lg font-bold tracking-wide text-slate-900">
                            TECHNICAL SKILLS
                        </h2>

                        <div className="mt-4 space-y-3">
                            {skills.length === 0 ? (
                                <p className="text-slate-500">Loading skills...</p>
                            ) : (
                                skills
                                    .sort((a, b) => a.displayOrder - b.displayOrder)
                                    .map((skill) => (
                                        <p key={skill.id} className="leading-7 text-slate-700">
                                            <span className="font-semibold text-slate-900">
                                                {skill.category}:
                                            </span>{" "}
                                            {skill.skills}
                                        </p>
                                    ))
                            )}
                        </div>
                    </section>

                    <section className="border-b border-slate-200 pb-6">
                        <h2 className="text-lg font-bold tracking-wide text-slate-900">
                            SELECTED TECHNICAL PROJECTS
                        </h2>

                        {projects.length === 0 ? (
                            <p className="mt-3 text-slate-500">Loading projects...</p>
                        ) : (
                            <div className="mt-5 space-y-4">
                                {projects
                                    .sort((a, b) => a.displayOrder - b.displayOrder)
                                    .map((project) => (
                                        <div
                                            key={project.id}
                                            className="rounded-2xl border border-slate-200 bg-slate-50 p-5"
                                        >
                                            <h3 className="text-lg font-semibold text-slate-900">
                                                {project.name}
                                            </h3>

                                            <p className="mt-2 text-sm text-slate-600">
                                                <span className="font-semibold">Tech:</span>{" "}
                                                {project.technologies}
                                            </p>

                                            <p className="mt-3 text-slate-700">
                                                {project.description}
                                            </p>
                                        </div>
                                    ))}
                            </div>
                        )}
                    </section>

                    <section className="border-b border-slate-200 pb-6">
                        <h2 className="text-lg font-bold tracking-wide text-slate-900">
                            PROFESSIONAL CERTIFICATIONS
                        </h2>

                        {certifications.length === 0 ? (
                            <p className="mt-3 text-slate-500">
                                Loading certifications...
                            </p>
                        ) : (
                            <ul className="mt-4 space-y-3 text-slate-700">
                                {certifications
                                    .sort((a, b) => a.displayOrder - b.displayOrder)
                                    .map((cert) => (
                                        <li key={cert.id}>
                                            {cert.name} - {cert.issuer} ({cert.yearObtained})
                                            {cert.credentialUrl && (
                                                <a
                                                    href={cert.credentialUrl}
                                                    target="_blank"
                                                    rel="noopener noreferrer"
                                                    className="ml-2 text-emerald-700 hover:underline"
                                                >
                                                    🔗 Verify
                                                </a>
                                            )}
                                        </li>
                                    ))}
                            </ul>
                        )}
                    </section>

                    <section className="border-b border-slate-200 pb-6">
                        <h2 className="text-lg font-bold tracking-wide text-slate-900">
                            EDUCATION & QUALIFICATIONS
                        </h2>

                        {education.length === 0 ? (
                            <p className="mt-3 text-slate-500">Loading education...</p>
                        ) : (
                            <div className="mt-4 space-y-3 text-slate-700">
                                {education.map((edu) => (
                                    <p key={edu.id}>
                                        {edu.institution} - {edu.degree}
                                        {edu.fieldOfStudy && ` (${edu.fieldOfStudy})`}
                                        {edu.grade && `, ${edu.grade}`}, {edu.yearGraduated}
                                    </p>
                                ))}
                            </div>
                        )}
                    </section>

                    <section className="border-b border-slate-200 pb-6">
                        <h2 className="text-lg font-bold tracking-wide text-slate-900">
                            PROFESSIONAL INTERESTS
                        </h2>

                        {interest.length === 0 ? (
                            <p className="mt-3 text-slate-500">Loading interests...</p>
                        ) : (
                            <div className="mt-4 space-y-2 text-slate-700">
                                {interest.map((item) => (
                                    <p key={item.id}>{item.description}</p>
                                ))}
                            </div>
                        )}
                    </section>

                    <section>
                        <h2 className="text-lg font-bold tracking-wide text-slate-900">
                            REFEREES
                        </h2>

                        {referee.length === 0 ? (
                            <p className="mt-3 text-slate-500">Loading referees...</p>
                        ) : (
                            <div className="mt-4 space-y-4 text-slate-700">
                                {referee.map((item) => (
                                    <div key={item.id}>
                                        {item.name === "Available on request" ? (
                                            <p>{item.name}</p>
                                        ) : (
                                            <div>
                                                <p className="font-semibold text-slate-900">
                                                    {item.name}
                                                </p>

                                                <p>
                                                    {item.title} at {item.company}
                                                </p>

                                                <p>
                                                    Email: {item.email} | Phone: {item.phone}
                                                </p>
                                            </div>
                                        )}
                                    </div>
                                ))}
                            </div>
                        )}
                    </section>

                    <div className="pt-4">
                        <button
                            onClick={(e) => {
                                e.preventDefault();

                                const link = document.createElement("a");
                                link.href = "/Future Ibeche_CV_.Net Dev.rc.pdf";
                                link.download = "Future_Ibeche_CV.pdf";

                                document.body.appendChild(link);
                                link.click();
                                document.body.removeChild(link);
                            }}
                            className="inline-flex items-center rounded-xl bg-emerald-600 px-5 py-3 font-semibold text-white transition hover:bg-emerald-700"
                        >
                            📥 Download PDF Resume
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default PublicResumePage;