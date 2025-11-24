"use client"; 

import React, { useState } from "react";
import axios from "../utils/api"; 
import styled from "styled-components";

// TypeScript type for the event
interface ChangeEvent {
  target: HTMLInputElement;
}

const Container = styled.div`
  padding: 20px;
`;

const Title = styled.h2`
  color: #333;
`;

const Button = styled.button`
  background-color: #007bff;
  color: white;
  padding: 10px 15px;
  border: none;
  cursor: pointer;
  &:disabled {
    background-color: #ccc;
  }
`;

const SkillList = styled.div`
  margin-top: 20px;
  ul {
    list-style-type: none;
  }
`;

const UploadForm: React.FC = () => {
  const [file, setFile] = useState<File | null>(null);
  const [status, setStatus] = useState<string>("");  // Upload status
  const [skills, setSkills] = useState<string[]>([]); // Extracted skills from the resume

  // Handler to upload the resume
  const uploadResume = async () => {
    if (!file) return;

    setStatus("Uploading...");

    const formData = new FormData();
    formData.append("file", file);

    try {
      const res = await axios.post("/resumes/upload", formData, {
        headers: { "Content-Type": "multipart/form-data" },
      });

      setStatus(`Upload success! ID: ${res.data.id}`);
      setSkills(res.data.skills); // Assuming your backend sends the extracted skills
    } catch (err: any) {
      console.error(err);
      setStatus("Upload failed");
    }
  };

  // Handle file input change
  const handleFileChange = (e: ChangeEvent) => {
    const selectedFile = e.target.files ? e.target.files[0] : null;
    setFile(selectedFile);
  };

  return (
    <Container>
      <Title>Upload Resume</Title>
      
      {/* File input */}
      <input
        type="file"
        accept=".pdf,.docx"
        onChange={handleFileChange}
      />
      <Button onClick={uploadResume} disabled={!file}>Upload</Button>

      {/* Status message */}
      {status && <p>{status}</p>}

      {/* Display detected skills */}
      {skills.length > 0 && (
        <SkillList>
          <h3>Detected Skills:</h3>
          <ul>
            {skills.map((skill, index) => (
              <li key={index}>{skill}</li>
            ))}
          </ul>
        </SkillList>
      )}
    </Container>
  );
};

export default UploadForm;
