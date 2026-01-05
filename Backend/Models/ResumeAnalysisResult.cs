using System;
using System.Collections.Generic;

namespace backend.Models
{
    public class ResumeAnalysisResult
    {
        public string ResumeFileName { get; set; } = "";
        public int AtsScore { get; set; }
        public string Summary { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        
        public List<string> Strengths { get; set; } = new List<string>();
        public List<string> Weaknesses { get; set; } = new List<string>();
        public List<string> Improvements { get; set; } = new List<string>();
    }
}
