AI Resume Analyzer





AI Resume Analyzer is a full-stack web application that allows users to upload resumes and receive AI-powered analysis, including skill extraction, job match scoring, and personalized improvement suggestions.



The project was built to demonstrate real-world full-stack engineering, backend API design, database persistence, authentication, and AI integration using modern technologies.









# Project Overview





Users can upload resumes (PDF or DOCX) through a modern frontend interface.

The backend processes the file, extracts structured data, analyzes it using AI, and stores results in a PostgreSQL database.



The application focuses on:



Clean API design
Secure file handling
AI-assisted data processing
Production-style frontend UX and error handling










# Tech Stack







Frontend





Next.js
React
TypeScript
Axios






Backend





.NET 8 Web API
ASP.NET Core
Entity Framework Core






Database





PostgreSQL (Render-hosted)






AI Integration





OpenAI API (resume analysis & suggestions)






Tooling





JWT authentication
Swagger API documentation










# Key Features





Resume upload (PDF & DOCX)
Secure, authenticated upload endpoints
AI-powered skill extraction
Job relevance and improvement suggestions
Persistent storage of resume metadata and analysis results
Clean UI with upload states, progress indicators, and validation
RESTful backend API with proper error handling










# Authentication





JWT-based authentication (register & login)
Protected endpoints for resume uploads
User-specific resume ownership and access control










# Development Highlights







Backend





REST API built with ASP.NET Core
Resume parsing using PDF and DOCX libraries
Modular services for parsing, skill extraction, and AI analysis
PostgreSQL schema managed via Entity Framework Core migrations
Deployed using Render with managed PostgreSQL






Frontend





Resume upload UI with validation (file type & size)
Clear success, error, and loading states
API integration using Axios
Clean separation of concerns between UI and data logic










# Known Limitations





Job match scoring is heuristic-based and can be further refined
No admin dashboard included
AI responses depend on OpenAI API configuration and prompt tuning










# Running the Project Locally







Prerequisites





.NET 8 SDK
Node.js & npm
PostgreSQL database (local or cloud)






Backend Setup



git clone https://github.com/SimbaChasumba1/ai-resume-analyzer.git

cd Backend

dotnet restore

dotnet ef database update

dotnet run



Frontend Setup



cd frontend

npm install

npm run dev

Create a .env file with:



Database connection string
JWT secret
OpenAI API key










# Notes





This project was built to reflect real production-style workflows rather than a demo or tutorial.

It emphasizes system design, API structure, AI integration, and maintainable full-stack code.