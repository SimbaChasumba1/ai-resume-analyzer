import * as React from 'react';
import axios from 'axios';

const BACKEND_URL = 'http://localhost:5240/upload';

export default function UploadForm() {
  const [file, setFile] = React.useState<File | null>(null);
  const [status, setStatus] = React.useState('');
  const inputRef = React.useRef<HTMLInputElement | null>(null);
  const [dragging, setDragging] = React.useState(false);

  const handleSelect = (f: File | null) => {
    setFile(f);
    if (f) setStatus(`Selected: ${f.name}`);
  };

  const upload = async () => {
    if (!file) return setStatus('Please select a PDF.');
    try {
      setStatus('Uploading…');
      const formData = new FormData();
      formData.append('file', file);
      const res = await axios.post(BACKEND_URL, formData, {
        headers: { 'Content-Type': 'multipart/form-data' },
        timeout: 15000,
      });
      // try to display server response (analysis)
      console.log('upload res', res.data);
      setStatus('Upload successful — analysis queued.');
    } catch (err) {
      console.error(err);
      setStatus('Upload failed — backend unreachable.');
    }
  };

  return (
    <div className="card upload-card">
      <h2 className="title">Upload Resume</h2>
      <div
        className={`dropzone ${dragging ? 'dragging' : ''}`}
        onClick={() => inputRef.current?.click()}
        onDrop={(e) => { e.preventDefault(); setDragging(false); handleSelect(e.dataTransfer.files[0] || null); }}
        onDragOver={(e) => { e.preventDefault(); setDragging(true); }}
        onDragLeave={() => setDragging(false)}
      >
        <input ref={inputRef} type="file" accept="application/pdf" className="hidden" onChange={e => handleSelect(e.target.files?.[0] || null)} />
        <p className="muted">{file ? file.name : 'Click or drag a PDF here to upload'}</p>
      </div>

      <div className="actions-row">
        <button onClick={upload} className="btn primary">Upload & Analyze</button>
        <button onClick={() => { setFile(null); setStatus(''); }} className="btn outline">Clear</button>
      </div>

      {status && <div className="status">{status}</div>}
    </div>
  );
}
