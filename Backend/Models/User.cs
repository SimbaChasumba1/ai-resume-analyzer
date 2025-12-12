using System;
using System.Collections.Generic;

namespace backend.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }


        public List<ResumeUpload> Resumes { get; set; } = new List<ResumeUpload>();
    }
}
