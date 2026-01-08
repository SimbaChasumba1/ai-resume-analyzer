using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace backend.Models
{
    public class ResumeAnalysisResult
    {
        [JsonPropertyName("resumeFileName")]
        public string ResumeFileName { get; set; } = "";

        [JsonPropertyName("atsScore")]
        public int AtsScore { get; set; }

        [JsonPropertyName("summary")]
        public string Summary { get; set; } = "";

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("strengths")]
        public List<string> Strengths { get; set; } = new List<string>();

        [JsonPropertyName("weaknesses")]
        public List<string> Weaknesses { get; set; } = new List<string>();

        [JsonPropertyName("improvements")]
        public List<string> Improvements { get; set; } = new List<string>();
    }
}
