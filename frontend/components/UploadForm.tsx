import React, { useState } from "react";
import axios from "../utils/api";
import styled from "styled-components";

// Styled Components
const FormContainer = styled.div`
  margin: 20px;
  padding: 20px;
  text-align: center;
  background-color: #f4f4f4;
  border-radius: 10px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
`;

const Heading = styled.h2`
  font-size: 24px;
  color: #333;
`;

const UploadButton = styled.button`
  margin-top: 10px;
  padding: 10px 20px;
  font-size: 16px;
  cursor: pointer;
  background-color: #007bff;
  color: white;
  border: none;
  border-radius: 5px;

  &:disabled {
    background-color: #ccc;
  }
`;

const FileInput = styled.input`
  margin-top: 20px;
  padding: 5px;
  font-size: 16px;
`;

const SkillList = styled.ul`
  list-style-type: none;
  padding: 0;
`;

const SkillItem = styled.li`
  font-size: 18px;
  color: #333;
`;

export default function UploadForm() {
  const [file, setFile] = useState<File | null>(null);
  const [status, setStatus] = useState<string>("");
  const [skills, setSkills] = useState<string[]>([]);

  const uploadResume = async () => {
    if (!file) {
      setStatus("Please select a file.");
      return;
    }

    if (!file.name.endsWith(".pdf") && !file.name.endsWith(".docx")) {
      setStatus("Invalid file format. Only PDF and DOCX are allowed.");
      return;
    }

    if (file.size > 5 * 1024 * 1024) { // 5MB
      setStatus("File size exceeds the 5MB limit.");
      return;
    }

    setStatus("Uploading...");

    const formData = new FormData();
    formData.append("file", file);

    try {
      const res = await axios.post("/resumes/upload", formData, {
        headers: { "Content-Type": "multipart/form-data" },
      });
      setStatus(`Upload success! ID: ${res.data.id}`);
      setSkills(res.data.skills); // Set extracted skills
    } catch (err) {
      if (err instanceof Error) {
        setStatus(`Upload failed: ${err.message}`);
      } else {
        setStatus("Upload failed.");
      }
    }
  };

  return (
    <FormContainer>
      <Heading>Upload Resume</Heading>
      <FileInput
        type="file"
        accept=".pdf,.docx"
        onChange={(e: React.ChangeEvent<HTMLInputElement>) => setFile(e.target.files?.[0] ?? null)}
      />
      <UploadButton onClick={uploadResume} disabled={!file}>
        Upload
      </UploadButton>
      {status && <p>{status}</p>}

      {skills.length > 0 && (
        <div>
          <h3>Detected Skills:</h3>
          <SkillList>
            {skills.map((skill, index) => (
              <SkillItem key={index}>{skill}</SkillItem>
            ))}
          </SkillList>
        </div>
      )}
    </FormContainer>
  );
}
