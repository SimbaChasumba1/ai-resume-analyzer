# AI Resume Analyzer

This project is a web application that allows users to upload resumes, extract key information (like skills), and analyze them using AI-powered tools. The backend handles file uploads, parsing resumes, and extracting skills, while the frontend provides a user interface for uploading resumes and displaying the results.

## Features

- Upload resumes in PDF or DOCX formats.
- Extract skills from uploaded resumes.
- View detected skills and the status of the upload.
- Backend powered by ASP.NET Core with Entity Framework for data management.
- Frontend built using React with TypeScript.

---

## Day 1 - Setup and Basic Backend

### Tasks Completed:

1. **Project Initialization:**
   - Created a new ASP.NET Core Web API project.
   - Set up basic project structure for the backend, including models and controllers.

2. **Resume Upload Endpoint:**
   - Created an endpoint `/resumes/upload` to handle file uploads.
   - Utilized `IFormFile` to accept files and store them in the server’s file system.
   - Saved uploaded resumes in the database (with metadata like file name and upload time).

3. **Database Setup:**
   - Configured Entity Framework Core to connect to a SQL Server database.
   - Created a basic `Resume` model to store resume metadata.
   - Set up the connection string and successfully ran migrations to create the database schema.

4. **Testing:**
   - Tested the backend file upload functionality and database interaction.
   - Verified that uploaded files are being saved correctly and metadata is stored in the database.

---

## Day 2 - Resume Parsing and Frontend Integration

### Tasks Completed:

1. **Resume Parsing Service:**
   - Integrated a resume parsing service to extract skills from the uploaded resumes.
   - Added logic to process the resume file and identify skills, storing these in the database.

2. **Frontend Development:**
   - Set up React project with TypeScript for the frontend.
   - Created an `UploadForm` component to allow users to upload resumes via the frontend.
   - Implemented the `uploadResume` function using Axios to send files to the backend for processing.

3. **Skill Extraction Display:**
   - After uploading a resume, the frontend displays a list of detected skills extracted from the resume.
   - Added status messages (e.g., "Uploading...", "Upload success!") to indicate the status of the upload.

4. **API Integration:**
   - Integrated the backend API with the frontend using Axios.
   - The `UploadForm` component successfully sends a POST request to the `/resumes/upload` endpoint and handles the response.

5. **Testing:**
   - Tested the frontend and backend interaction.
   - Verified that the file upload works, the backend processes the file, and the frontend displays the extracted skills.

---

## Day 3 - Auth, Improved Upload UI & DB persistence

### Tasks Completed:
- Implemented JWT-based auth (register/login) so users can upload resumes to a protected endpoint.
- Improved Upload UI: drag & drop, validation (PDF/DOCX, max 5MB), progress bar, nicer styling using styled-components.
- Backend: Save resume metadata to DB (Resumes table). Resume parsing uses PDF and DOCX libs and a SkillExtractor service.
- Added migrations commands and instructions.
- UI images and brand polish included (see `/frontend/public` or referenced local assets).


## Tech Stack

- **Backend:**
  - ASP.NET Core
  - Entity Framework Core
  - SQL Server (for database storage)
  - Swagger (for API documentation)

- **Frontend:**
  - React
  - TypeScript
  - Axios (for API requests)

---

## Setup Instructions

### Prerequisites

- .NET SDK (for running the backend)
- Node.js and npm (for running the frontend)
- SQL Server (or another database of choice) with the appropriate connection string

### Backend Setup

1. Clone the repository:
   ```bash
   git clone <repository-url>
