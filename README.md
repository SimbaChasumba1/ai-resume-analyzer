# AI Resume Analyzer

AI Resume Analyzer is a full-stack web application that allows users to upload resumes and receive AI-powered analysis, including skill extraction, job match scoring, and personalized improvement suggestions.

The project demonstrates **real-world full-stack engineering**, including backend API design, authentication, database persistence, and AI integration using modern technologies.


# Live Demo

Frontend: https://ai-resume-analysis-platform.vercel.app



# Project Overview

Users upload resumes (PDF) through a modern frontend interface.

The backend:

- Processes uploaded files  
- Extracts structured resume data  
- Sends content to an AI service for analysis  
- Stores results in a PostgreSQL database  
- Returns structured feedback to the user  

The application focuses on:

- Clean REST API design  
- Secure file handling  
- AI-assisted data processing  
- Production-style frontend UX  
- Reliable error handling  


# Tech Stack

## Frontend

- Next.js  
- React  
- TypeScript  
- Axios  

## Backend

- .NET 8 Web API  
- ASP.NET Core  
- Entity Framework Core  

## Database

- PostgreSQL (Render-hosted)

## AI Integration

- OpenAI API (resume analysis & suggestions)

## Tooling

- JWT Authentication  
- Swagger API Documentation  


# Key Features

- Resume upload (PDF & DOCX support)  
- Secure authenticated upload endpoints  
- AI-powered skill extraction  
- Job relevance scoring  
- Personalized improvement suggestions  
- Persistent storage of resume metadata  
- Clean UI with upload progress indicators  
- RESTful backend API with structured error handling  


# Authentication

- JWT-based authentication (register & login)  
- Protected upload endpoints  
- User-specific resume ownership  
- Access-controlled resume retrieval  


# Development Highlights

## Backend

- REST API built using ASP.NET Core  
- Resume parsing using PDF and DOCX libraries  
- Modular services for parsing, skill extraction, and AI analysis  
- PostgreSQL schema managed via Entity Framework Core migrations  
- Production deployment using Render  

## Frontend

- Resume upload UI with validation (file type & size)  
- Clear loading, success, and error states  
- API communication using Axios  
- Clean separation between UI components and data logic  


# Known Limitations

- Job match scoring is heuristic-based and can be further refined  
- No admin dashboard included  
- AI responses depend on OpenAI API configuration and prompt tuning  


# Running the Project Locally

## Prerequisites

- .NET 8 SDK  
- Node.js & npm  
- PostgreSQL database  


## Backend Setup

```bash
git clone https://github.com/SimbaChasumba1/ai-resume-analyzer.git

cd Backend

dotnet restore

dotnet ef database update

dotnet run