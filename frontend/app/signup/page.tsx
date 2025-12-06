export default function SignupPage() {
  return (
    <div className="flex flex-col items-center justify-center mt-24 px-4">
      <h1 className="text-3xl font-semibold mb-6">Sign Up</h1>

      <form className="bg-[#141417] p-8 rounded-xl border border-[#2a2a2d] w-full max-w-md">
        <input
          type="text"
          placeholder="Full Name"
          className="w-full p-3 mb-4 rounded-lg bg-[#0F0F11] border border-[#2a2a2d]"
        />
        <input
          type="email"
          placeholder="Email"
          className="w-full p-3 mb-4 rounded-lg bg-[#0F0F11] border border-[#2a2a2d]"
        />
        <input
          type="password"
          placeholder="Password"
          className="w-full p-3 mb-6 rounded-lg bg-[#0F0F11] border border-[#2a2a2d]"
        />

        <button className="w-full py-3 rounded-lg bg-[#6A5ACD] hover:bg-[#7d6dff]">
          Create Account
        </button>
      </form>
    </div>
  );
}
