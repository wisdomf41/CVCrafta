import React from 'react';
import React, { useEffect, useState } from 'react';
import './App.css';

function App() {

    const [resumeData, setResumeData] = useState(null);

    useEffect(() => { 
        // Fetch resume data from the backend API
        fetch("https://myresumefi.azurewebsites.net/api/resume")
            .then(response => response.json())
            .then(data => {
                console.log(data);
                setResume(data[0]); // Because the API returns an array, we take the first item

            });
    }, []);

    if (!resume) return <p>Loading...</p>;

    return (
        <div className="resume-container">
            {/* Header Section */}
            <header className="header">
                <h1> {resume.name} </h1>
                <h1> {resume.title} </h1>
                <h1> React | SQL | ASP.NET Core | Rest API</h1>
                <div className="contact">
                    <span>📧 {resume.email}</span>
                    <span>🔗 GitHub / LinkedIn (@wisdomf41)</span>
                    <span>📍 Nigeria</span>
                </div>
            </header>

            {/* Summary Section */}
            <section className="section">
                <h2>SUMMARY</h2>
                <p>
                    <p>
                        Full-Stack .NET Developer with experience in building full-stack applications using C#,
                        ASP.NET Core Web API, and React. Skilled in developing RESTful APIs, SQL Server,
                        and Entity Framework Core, with a focus on performance and scalability.
                        Experienced in implementing CI/CD pipelines (GitHub Actions/Azure DevOps) to automate
                        builds and deployments. Strong collaborator in Agile and remote teams, passionate about
                        building efficient, maintainable systems.
                    </p>
                </p>
            </section>

            {/* Experience Section */}
            <section className="section">
                <h2>EXPERIENCE</h2>

                <div className="job">
                    <div className="job-header">
                        <h3>
                            Full-Stack .NET Developer
                            <span className="date">2024 - 2026</span>

                        </h3>
                        {/* <span className="date">2024 - 2025</span> */}
                    </div>
                    <p className="company">Metclan Technologies</p>
                    <ul>
                        <li>Developed 3+ full-stack applications using <strong>ASP.NET Core Web API</strong> and <strong>React</strong>.</li>
                        <li>Built reusable React components, reducing development time by 30%.</li>
                        <li>Integrated REST APIs and optimized queries with Entity Framework Core, improving performance by 25%.</li>
                        <li>Implemented CI/CD pipelines (GitHub Actions/Azure DevOps) to automate build and deployment.</li>
                        <li>Collaborated in an Agile team, using Git for version control..</li>
                    </ul>
                </div>

                <div className="job">
                    <div className="job-header">
                        <h3>
                            Software Developer
                            <span className="date">2024 - 2025</span>

                        </h3>

                    </div>

                    <p className="company">Loctech Training Institute</p>
                    <ul>
                        <li>Developed applications using .NET, C#, HTML, CSS, JavaScript, and jQuery.</li>
                        <li>Contributed to the development of a healthcare management system, improving patient record handling and data accessibility.</li>
                        <li>Diagnosed and resolved 20+ software bugs, enhancing system reliability and user satisfaction.</li>

                        {/* Test CI/CD - Update */}
                        <li>Assisted in application testing, debugging, and deployment processes.</li>
                    </ul>
                </div>

                <div className="job">
                    <div className="job-header">
                        <h3>
                            IT/Network Specialist
                            <span className="date">2021 - 2024</span>

                        </h3>

                    </div>

                    <p className="company">FirstBank limited</p>
                    <ul>
                        <li>Installed, customized, and supported desktop, laptop workstations, and software systems for 100+ in-office and remote work users.</li>
                        <li>Refurbished 100+ Windows 8 Machines to Windows 10 over four days in preparation for an Office 365 roll-out across the eight regional offices.</li>
                        <li>Mentored and supervised 5+ junior team members, and controlled training and information technology systems.</li>
                        <li>Managed migration of data storage and backup systems to cloud, enhancing security and disaster recovery.</li>
                        <li>Provided onsite IT support for between 20 and 40 calls on a daily basis.</li>
                    </ul>
                </div>
            </section>

            {/* Technical Skills */}
            <section className="section">
                <h2>TECHNICAL SKILLS</h2>
                <div className="skills-list">
                    <p><strong>Backend:</strong> C#, .NET Core Web API, REST APIs</p>
                    <p><strong>Database:</strong> SQL Server, Entity Framework Core, LINQ</p>
                    <p><strong>Frontend:</strong> React, HTML 5, CSS3, JavaScript (ES6+), Vite</p>
                    <p><strong>Tools:</strong> Git, GitHub, Visual Studio, Azure</p>
                    <p><strong>Concepts:</strong> Full-Stack Develpment, API Integration, Component-Based Architecture, Authentication & Authorization</p>
                </div>
            </section>

            {/* Projects */}
            <section className="section">
                <h2>SELECTED TECHNICAL PROJECTS</h2>


                <ul>

                    <li><strong>Resume Website (This Project)</strong> - <h5><strong>Tech: </strong> React + Vite frontend, ASP.NET Core backend, Azure App Service :</h5> Full-stack resume application with responsive design, deployed to Azure cloud.</li>
                    <li><strong>Task Management API with React Frontend</strong> - <h5><strong>Tech: </strong> ASP.NET Core Web API, React, SQL Server, Entity Framework :</h5> ASP.NET Core Web API, React, SQL Server, Entity Framework, Full-stack task management system with CRUD operations, authentication, and responsive UI.</li>
                    <li><strong>E-Commerce MVC Application</strong> - <h5><strong>Tech: </strong> ASP.NET Core MVC, HTML 5, CSS 3, JavaScript, SQL Server :</h5> Complete e-commerce solution with product management, shopping cart, and order processing.</li>
                    <li><strong>Hotel Booking API</strong> - <h5><strong>Tech: </strong>ASP.NET Core Web API, SQL Server, Entity Framework :</h5> RESTful API for hotel room booking with customer management and reservation tracking.</li>
                </ul>
            </section>

            {/* Certifications */}
            <section className="section">
                <h2>PROFESSIONAL CERTIFICATIONS</h2>
                <ul>
                    <li>.NET Core Developer Certification - Udemy (2025)</li>
                    <li>.NET Core API Developer Certification - Udemy (2025)</li>
                    <li>Google IT Support Professional Certificate - Coursera (2024)</li>
                    <li>SQL for Data Analysis - Udacity (2022)</li>
                </ul>
            </section>

            {/* Education */}
            <section className="section">
                <h2>EDUCATION & QUALIFICATIONS</h2>
                <p>University of Port-Harcourt - Bachelor of Engineering (B.Eng., 2nd Class Upper Division). 2018</p>
            </section>

            {/* Professional Interests */}
            <section className="section">
                <h2>PROFESSIONAL INTERESTS</h2>
                <p>Continuous learning in the .NET ecosystem. Building scalable and maintainable software solutions.</p>
            </section>

            {/* Referees */}
            <section className="section">
                <h2>REFEREES</h2>
                <p>Available on request.</p>
            </section>

            {/* Download PDF Button */}
            <div className="download-section">
                <a href="/Future Ibeche_CV_.Net Dev.rc.pdf" download className="download-btn">
                    📥 Download PDF Resume
                </a>
            </div>
        </div>
    );
}

export default App;