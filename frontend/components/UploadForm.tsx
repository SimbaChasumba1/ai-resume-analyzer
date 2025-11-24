"use client";

import React, { useState, useRef, useEffect } from "react";
import type { AxiosProgressEvent } from "axios";  
import axios from "../utils/api";             
import styled from "styled-components";

const Container = styled.div`
  max-width: 760px;
  margin: 24px auto;
  background: #ffffff;
  border-radius: 12px;
  padding: 24px;
  box-shadow: 0 6px 18px rgba(20,20,30,0.06);
`;

const Title = styled.h2`
  color: #111827;
  margin-bottom: 8px;
`;

const DropArea = styled.label<{ isDragging: boolean }>`
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 12px;
  padding: 28px;
  border: 2px dashed ${p => (p.isDragging ? "#2563eb" : "#e5e7eb")};
  background: ${p => (p.isDragging ? "rgba(37,99,235,0.04)" : "transparent")};
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.15s ease;
`;

const Controls = styled.div`
  display: flex;
  gap: 12px;
  margin-top: 16px;
`;

const Button = styled.button`
  background-color: #2563eb;
  color: white;
  padding: 10px 16px;
  border: none;
  cursor: pointer;
  border-radius: 8px;
  font-weight: 600;
  &:disabled {
    opacity: 0.6;
    cursor: not-allowed;
  }
`;

const Text = styled.p`
  margin: 0;
  color: #6b7280;
`;

const ProgressBar = styled.div`
  height: 8px;
  background: #f3f4f6;
  border-radius: 999px;
  overflow: hidden;
  margin-top: 12px;
`;

const Progress = styled.div<{ pct: number }>`
  height: 100%;
  width: ${p => p.pct}%;
  background: linear-gradient(90deg, #2563eb, #60a5fa);
  transition: width 0.25s ease;
`;

const SkillList = styled.div`
  margin-top: 20px;
  ul {
    display: flex;
    flex-wrap: wrap;
    gap: 8px;
    padding: 0;
    margin: 8px 0 0 0;
    list-style: none;
  }
  li {
    background: #f8fafc;
    padding: 6px 10px;
    border-radius: 999px;
    color: #0f172a;
    font-weight: 600;
    font-size: 13px;
  }
`;

export default function UploadForm() {
  const [file, setFile] = useState<File | null>(null);
  const [status, setStatus] = useState<string>("");
  const [skills, setSkills] = useState<string[]>([]);
  const [isDragging, setIsDragging] = useState(false);
  const [progress, setProgress] = useState(0);
  const inputRef = useRef<HTMLInputElement | null>(null);

  useEffect(() => {
    if (progress === 100) setTimeout(() => setProgress(0), 600);
  }, [progress]);

  const handleFile = (f: File | null) => {
    if (!f) return setFile(null);
    const allowed = [
      "application/pdf",
      "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
    ];
    if (!allowed.includes(f.type)) {
      setStatus("Invalid file type. Please upload a PDF or DOCX.");
      setFile(null);
      return;
    }
    if (f.size > 5 * 1024 * 1024) {
      setStatus("File too large. Max 5MB.");
      setFile(null);
      return;
    }
    setStatus("");
    setFile(f);
  };

  const onDrop = (e: React.DragEvent<HTMLLabelElement>) => {
    e.preventDefault();
    setIsDragging(false);
    const f = e.dataTransfer.files[0];
    handleFile(f || null);
  };

  const uploadResume = async () => {
    if (!file) return;
    setStatus("Uploading...");
    setSkills([]);
    setProgress(6);

    const formData = new FormData();
    formData.append("file", file);

    try {
      const res = await axios.post("/resumes/upload", formData, {
        headers: { "Content-Type": "multipart/form-data" },
        onUploadProgress: (progressEvent: AxiosProgressEvent) => {
          if (progressEvent.total) {
            const pct = Math.round((progressEvent.loaded / progressEvent.total) * 100);
            setProgress(pct);
          }
        },
      });

      setStatus("Upload success!");
      setSkills(res.data.skills || []);
      setProgress(100);
    } catch (err: any) {
      console.error(err?.response || err);
      setStatus(err?.response?.data || "Upload failed");
      setProgress(0);
    }
  };

  return (
    <Container>
      <Title>Upload resume</Title>
      <DropArea
        htmlFor="file-input"
        isDragging={isDragging}
        onDragOver={(e) => { e.preventDefault(); setIsDragging(true); }}
        onDragLeave={() => setIsDragging(false)}
        onDrop={onDrop}
      >
        <div style={{ textAlign: "center" }}>
          <strong>{file ? file.name : "Drag & drop your resume here"}</strong>
          <div style={{ marginTop: 6 }}>
            <Text>{file ? `${Math.round(file.size / 1024)} KB` : "PDF or DOCX — up to 5MB"}</Text>
          </div>
        </div>
        <input
          id="file-input"
          ref={inputRef}
          type="file"
          accept=".pdf,.docx"
          style={{ display: "none" }}
          onChange={(e) => handleFile(e.target.files ? e.target.files[0] : null)}
        />
      </DropArea>

      <Controls>
        <Button onClick={() => inputRef.current?.click()}>Choose file</Button>
        <Button onClick={uploadResume} disabled={!file}>
          Upload
        </Button>
      </Controls>

      {status && <p style={{ marginTop: 12 }}>{status}</p>}

      <ProgressBar aria-hidden>
        <Progress pct={progress} />
      </ProgressBar>

      {skills.length > 0 && (
        <SkillList>
          <h4>Detected skills</h4>
          <ul>
            {skills.map((s, i) => (
              <li key={i}>{s}</li>
            ))}
          </ul>
        </SkillList>
      )}
    </Container>
  );
}

