import React, { useEffect, useState } from 'react';
import './App.css';

function App() {
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
                //Fetch Resume
                const resumeResponse = await fetch("https://localhost:7085/api/resume");

                if (!resumeResponse.ok) {
                    throw new Error(`HTTP error! status: ${resumeResponse.status}`);
                }

                const resumeData = await resumeResponse.json();
                console.log("Resume data:", resumeData);

                if (resumeData && resumeData.length > 0) {
                    setResume(resumeData[0]);
                } else {
                    setError("No resume data found");
                    return;
                }

                //Fetch Experience
                const experienceResponse = await fetch("https://localhost:7085/api/Experience");

                if (!experienceResponse.ok) {
                    throw new Error(`HTTP error! status: ${experienceResponse.status}`);
                }

                const experienceData = await experienceResponse.json();
                console.log("Experience data:", experienceData);

                if (Array.isArray(experienceData)) {
                    setExperience(experienceData);
                } else {
                    setExperience([]);
                }

                //Fetch Skill
                const skillsResponse = await fetch("https://localhost:7085/api/Skill");
                if (!skillsResponse.ok) throw new Error(`HTTP error! status: ${skillsResponse.status}`);
                const skillsData = await skillsResponse.json();
                setSkills(skillsData);

                //Fetch Projects
                const projectsResponse = await fetch("https://localhost:7085/api/Project");
                if (!projectsResponse.ok) throw new Error(`HTTP error! status: ${projectsResponse.status}`);

                const projectsData = await projectsResponse.json();
                setProjects(projectsData);

                //Fetch Certifications
                const certificationsResponse = await fetch("https://localhost:7085/api/Certification");
                if (!certificationsResponse.ok) throw new Error(`HTTP error! status: ${certificationsResponse.status}`);

                const certificationsData = await certificationsResponse.json();
                setCertifications(certificationsData);

                //Fetch Education
                const educationResponse = await fetch("https://localhost:7085/api/Education");
                if (!educationResponse.ok) throw new Error(`HTTP error! status: ${educationResponse.status}`);

                const educationData = await educationResponse.json();
                setEducation(educationData);

                //Fetch Interest
                const interestResponse = await fetch("https://localhost:7085/api/Interest");
                if (!interestResponse.ok) throw new Error(`HTTP error! status: ${interestResponse.status}`);

                const interestsData = await interestResponse.json();
                setInterest(interestsData);


                //Fetch Referee
                const refereeResponse = await fetch("https://localhost:7085/api/Referee");
                if (!refereeResponse.ok) throw new Error(`HTTP error! status: ${refereeResponse.status}`);

                const refereeData = await refereeResponse.json();
                setReferee(refereeData);


            } catch (error) {
                console.error("Error fetching data:", error);
                setError(error.message);
            }
        };

        fetchData();
    }, []);

    if (error) {
        return (
            <div style={{ padding: "20px", textAlign: "center" }}>
                <h3>Error Loading Resume</h3>
                <p>{error}</p>
                <p>Make sure your database has resume data and backend is running.</p>
                <button onClick={() => window.location.reload()}>Retry</button>
            </div>
        );
    }

    if (!resume) return <p>Loading resume...</p>;

    return (
        <div className="resume-container">
            <header className="header">
                <h1>{resume.name}</h1>
                <h1>{resume.title}</h1>
                <h1>{resume.stack}</h1>
                <div className="contact">
                    <span>📧 {resume.email}</span>
                    <span>🔗 {resume.url}</span>
                    <span>📍 {resume.country}</span>
                </div>
            </header>

            <section className="section">
                <h2>SUMMARY</h2>
                <p>{resume.summary}</p>
            </section>

            <section className="section">
                <h2>EXPERIENCE</h2>

                {experience.length > 0 ? (
                    experience.map((item) => (
                        <div className="job" key={item.id}>
                            <div className="job-header">
                                <h3>
                                    {item.role}
                                    <span className="date">{item.duration}</span>
                                </h3>
                            </div>

                            <p className="company">{item.company}</p>

                            <ul>
                                {item.description?.split("|").map((point, index) => (
                                    <li key={index}>{point.trim()}</li>
                                ))}
                            </ul>
                        </div>
                    ))
                ) : (
                    <p>No experience data found.</p>
                )}
            </section>

            {/* Technical Skills - DYNAMIC from API */}
            <section className="section">
                    <h2>TECHNICAL SKILLS</h2>
                    <div className="skills-list">
                        {skills.length === 0 ? (
                            <p>Loading skills...</p>
                        ) : (
                            skills
                                .sort((a, b) => a.displayOrder - b.displayOrder) // Sort by order
                                .map((skill) => (
                                    <p key={skill.id}>
                                        <strong>{skill.category}:</strong> {skill.skills}
                                    </p>
                                ))
                        )}
                    </div>
              
            </section>

            {/* Selected Technical Projects - DYNAMIC from API */}
            <section className="section">
                    <h2>SELECTED TECHNICAL PROJECTS</h2>

                    {projects.length === 0 ? (
                        <p>Loading projects...</p>
                    ) : (
                        <ul>
                            {projects
                                .sort((a, b) => a.displayOrder - b.displayOrder)
                                .map((project) => (
                                    <li key={project.id}>
                                        <strong>{project.name}</strong> -
                                        <h5><strong>Tech:</strong> {project.technologies} :</h5>
                                        {project.description}
                                    </li>
                                ))}
                        </ul>
                    )}
            </section>

            {/* Certifications Section - DYNAMIC from API */}
            <section className="section">
                <h2>PROFESSIONAL CERTIFICATIONS</h2>
                {certifications.length === 0 ? (
                    <p>Loading certifications...</p>
                ) : (
                    <ul>
                        {certifications
                            .sort((a, b) => a.displayOrder - b.displayOrder)
                            .map((cert) => (
                                <li key={cert.id}>
                                    {cert.name} - {cert.issuer} ({cert.yearObtained})
                                    {cert.credentialUrl && (
                                        <a href={cert.credentialUrl} target="_blank" rel="noopener noreferrer">
                                            {" "}🔗 Verify
                                        </a>
                                    )}
                                </li>
                            ))}
                    </ul>
                )}
            </section>

            <section className="section">
                <h2>EDUCATION & QUALIFICATIONS</h2>
                {education.length === 0 ? (
                    <p>Loading education...</p>
                ) : (
                    education.map((edu) => (
                        <div key={edu.id}>
                            <p>
                                {edu.institution} - {edu.degree}
                                {edu.fieldOfStudy && ` (${edu.fieldOfStudy})`}
                                {edu.grade && `, ${edu.grade}`}, {edu.yearGraduated}
                            </p>
                        </div>
                    ))
                )}
            </section>

            <section className="section">
                <h2>PROFESSIONAL INTERESTS</h2>
                {interest.length === 0 ? (
                    <p>Loading interests...</p>
                ) : (
                    interest.map((interest) => (
                        <p key={interest.id}>{interest.description}</p>
                    ))
                )}
            </section>

            <section className="section">
                <h2>REFEREES</h2>
                {/*<p>Available on request.</p>*/}
                {referee.length === 0 ? (
                    <p>Loading referees...</p>
                ) : (
                    referee.map((referee) => (
                        <div key={referee.id}>
                            {referee.name === "Available on request" ? (
                                <p>{referee.name}</p>
                            ) : (
                                <div>
                                    <p><strong>{referee.name}</strong></p>
                                    <p>{referee.title} at {referee.company}</p>
                                    <p>Email: {referee.email} | Phone: {referee.phone}</p>
                                </div>
                            )}
                        </div>
                    ))
                )}
            </section>

            <div className="download-section">
                <button
                    onClick={(e) => {
                        e.preventDefault();
                        const link = document.createElement('a');
                        link.href = "/Future Ibeche_CV_.Net Dev.rc.pdf";
                        link.download = "Future_Ibeche_CV.pdf";
                        document.body.appendChild(link);
                        link.click();
                        document.body.removeChild(link);
                    }}
                    className="download-btn"
                >
                    📥 Download PDF Resume
                </button>
            </div>

        </div>
    );
}

export default App;

