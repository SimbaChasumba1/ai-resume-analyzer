import React, { useState } from "react";

import axios from "../../utils/api";





export default function UploadForm() {

  const [file, setFile] = useState<File | null>(null);

  const [status, setStatus] = useState("");



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

    } catch (err) {

      console.error(err);

      setStatus("Upload failed");

    }

  };



  return (

    <div style={{ padding: "20px" }}>

      <h2>Upload Resume</h2>

      <input type="file" accept=".pdf,.docx" onChange={e => setFile(e.target.files?.[0] ?? null)} />

      <button onClick={uploadResume} disabled={!file}>Upload</button>

      {status && <p>{status}</p>}

    </div>

  );

}