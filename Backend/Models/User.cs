using System;
using System.Collections.Generic;

namespace backend.Models
{
    public class User
    {
        public Guid Id { get; set; }


        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        // Navigation  
        public List<ResumeUpload> ResumeUploads { get; set; } = new();
    }
}
