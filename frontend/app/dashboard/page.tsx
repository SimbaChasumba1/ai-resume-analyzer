import AuthGuard from "@/components/AuthGuard";


export default function Dashboard() {
return (
<AuthGuard>
<div className="p-8 text-white">
<h1 className="text-3xl font-bold">Dashboard</h1>
<p className="text-slate-400 mt-2">Your resume insights</p>
</div>
</AuthGuard>
);
}