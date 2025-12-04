import * as React from 'react';
import axios from 'axios';

type Role = 'user' | 'assistant';
type Message = { role: Role; content: string };

export default function ChatPage() {
  const [messages, setMessages] = React.useState<Message[]>([
    { role: 'assistant', content: 'Hi — I can analyze resumes or answer questions. Upload one to start.' }
  ]);
  const [input, setInput] = React.useState('');

  const send = async () => {
    if (!input.trim()) return;
    const userMsg: Message = { role: 'user', content: input };
    setMessages(prev => [...prev, userMsg]);
    setInput('');

    // send to backend /api/chat or your endpoint
    try {
      const res = await axios.post('http://localhost:5240/api/chat', { prompt: userMsg.content }, { timeout: 15000 });
      const assistant = res.data?.reply ?? 'No reply from server';
      setMessages(prev => [...prev, { role: 'assistant', content: assistant }]);
    } catch (err) {
      setMessages(prev => [...prev, { role: 'assistant', content: 'Error: backend unreachable.' }]);
      console.error(err);
    }
  };

  return (
    <div className="chat-shell">
      <div className="chat-window">
        {messages.map((m, i) => (
          <div key={i} className={`chat-row ${m.role === 'user' ? 'user' : 'assistant'}`}>
            <div className="chat-bubble">{m.content}</div>
          </div>
        ))}
      </div>

      <div className="chat-input">
        <textarea value={input} onChange={e => setInput(e.target.value)} placeholder="Ask the AI..." />
        <button onClick={send} className="btn primary">Send</button>
      </div>
    </div>
  );
}
