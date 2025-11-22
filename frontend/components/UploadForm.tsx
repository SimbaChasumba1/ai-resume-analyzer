"use client"; // Ensure this is a client component

import React, { useState } from "react";
import axios from "../utils/api"; // Assuming you have an axios instance setup for API calls

// UploadForm Component
export default function UploadForm() {
  const [file, setFile] = useState<File | null>(null);
  const [status, setStatus] = useState<string>(""); // State to hold status message
  const [skills, setSkills] = useState<string[]>([]); // State to store detected skills

  // Function to handle file upload
  const uploadResume = async () => {
    if (!file) return; // If no file selected, exit early

    setStatus("Uploading..."); // Show upload status

    const formData = new FormData(); // Create a form data object to send the file
    formData.append("file", file);

    try {
      // Make the POST request to the backend
      const res = await axios.post("/resumes/upload", formData, {
        headers: { "Content-Type": "multipart/form-data" },
      });

      // On success, set the status and skills
      setStatus(`Upload success! ID: ${res.data.id}`);
      setSkills(res.data.skills); // Assuming backend returns skills in the response

    } catch (err) {
      // Handle errors appropriately
      if (err instanceof Error) {
        console.error("Upload failed:", err.message);
        setStatus(`Upload failed: ${err.message}`);
      } else {
        console.error("Unknown error:", err);
        setStatus("Upload failed. Unknown error.");
      }
    }
  };

  return (
    <div style={{ padding: "20px" }}>
      <h2>Upload Resume</h2>

      {/* File input */}
      <input
        type="file"
        accept=".pdf,.docx"
        onChange={(e) => setFile(e.target.files?.[0] ?? null)}
      />

      {/* Upload button */}
      <button onClick={uploadResume} disabled={!file}>
        Upload
      </button>

      {/* Display upload status */}
      {status && <p>{status}</p>}

      {/* Display detected skills (if any) */}
      {skills.length > 0 && (
        <div>
          <h3>Detected Skills:</h3>
          <ul>
            {skills.map((skill) => (
              <li key={skill}>{skill}</li>
            ))}
          </ul>
        </div>
      )}
    </div>
  );
}