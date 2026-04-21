# AI Resume Analyzer
AI Resume Analyzer is a full-stack AI-powered resume analysis platform that allows users to upload resumes and receive structured feedback, ATS scoring, and improvement suggestions.
The system demonstrates real-world distributed backend architecture by combining ASP.NET Core APIs with a Python FastAPI microservice powered by Groq LLMs and Redis caching.

# Live Demo
https://ai-resume-analysis-platform.vercel.app

# Video Walkthrough
https://simbachasumba.vercel.app/work/ai-resume-analyzer

# System Architecture Overview
Users upload resumes through the frontend interface.
The backend system:
- Receives uploaded resume files  
- Extracts text from PDF and DOCX documents  
- Sends extracted text to a Python FastAPI AI microservice  
- Uses Groq LLMs to analyze resume content  
- Caches AI responses using Redis to improve performance  
- Stores results in a PostgreSQL database  
- Returns structured feedback to the frontend dashboard  
This architecture demonstrates:
- Distributed backend services  
- Microservice-based AI integration  
- API-to-service communication  
- Performance optimization with caching  
- Secure authentication workflows  

# Tech Stack
## Frontend
- Next.js  
- React  
- TypeScript  
- Tailwind CSS  
- Axios  

## Backend (Main API)
- ASP.NET Core (.NET 8 Web API)  
- Entity Framework Core  
- REST API Architecture  
- JWT Authentication  

## AI Microservice
- Python  
- FastAPI  
- Groq LLM API  
- Redis (Response Caching)

## Database
- PostgreSQL (Render-hosted)

## Cloud & Deployment
- Render (Backend, Python Service, PostgreSQL, Redis)  
- Vercel (Frontend Hosting)  
- Swagger (API Testing & Documentation)

# Key Features
- Resume upload (PDF and DOCX support)  
- Secure authenticated file uploads  
- AI-powered resume analysis  
- ATS-style scoring system  
- Strength and weakness detection  
- Personalized improvement suggestions  
- Redis caching for faster repeated analysis  
- Persistent storage of analysis history  
- Structured REST API responses  
- Responsive dashboard UI  

# Authentication
- JWT-based authentication  
- Secure login and registration  
- Protected API endpoints  
- User-specific resume ownership  
- Access-controlled data retrieval  

# Backend Workflow
1. User uploads resume  
2. Backend extracts text  
3. Backend sends text to Python AI service  
4. Python service checks Redis cache  
5. If cached, return response  
6. If not cached, call Groq LLM  
7. Response stored in PostgreSQL  
8. Results returned to frontend  

# Development Highlights
## ASP.NET Core Backend
- Modular service-based architecture  
- Secure file upload handling  
- Resume parsing using PDF extraction libraries  
- HTTP client integration with Python microservice  
- PostgreSQL schema management using EF Core migrations  
- Production deployment using Render  

## Python AI Microservice
- Built with FastAPI  
- Uses Groq LLMs for resume analysis  
- Implements Redis caching for performance optimization  
- Returns structured JSON responses  
- Designed for independent scaling  

## Frontend
- Resume upload UI with validation  
- Loading indicators and error states  
- Dashboard displaying ATS scores and insights  
- API integration using Axios  
- Clean component-based architecture  

# Known Limitations
- Resume scoring logic can be further refined  
- AI output quality depends on prompt tuning  
- No admin-level dashboard implemented  
- Large file uploads may increase processing time  

# Running the Project Locally
## Prerequisites
- .NET 8 SDK  
- Node.js and npm  
- Python 3.11 or higher  
- PostgreSQL  
- Redis  

## Backend Setup
```bash
git clone https://github.com/SimbaChasumba1/ai-resume-analyzer.git
cd backend
dotnet restore
dotnet ef database update
dotnet run


Python Service Setup

cd python-service
pip install -r requirements.txt
uvicorn app.main:app --reload



Frontend Setup

cd frontend
npm install
npm run dev


Production Deployment

Frontend:

* Deployed on Vercel

Backend:

* Deployed on Render

Python AI Service:

* Deployed on Render

Database:

* PostgreSQL hosted on Render

Cache:

* Redis instance hosted on Render


Project Value

This project demonstrates:

* Full-stack engineering capability
* Distributed microservice architecture
* AI-powered backend systems
* Production deployment workflows
* Performance optimization using caching
* Secure authentication systems


Author

Simba Chasumba

Full-stack developer specializing in:

* ASP.NET Core backend systems
* Python AI microservices
* Distributed system architecture
* Full-stack application development

Portfolio:
https://simbachasumba.vercel.app