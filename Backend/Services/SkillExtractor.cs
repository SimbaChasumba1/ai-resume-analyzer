namespace Backend.Services{

public class SkillExtractor
{
    private readonly string[] skills = new[] { "C#", "Python", "JavaScript", "SQL", "React", "ASP.NET", "Node.js" };

    public List<string> Extract(string resumeText)
    {
        var found = new List<string>();
        foreach (var skill in skills)
        {
            if (resumeText.Contains(skill, StringComparison.OrdinalIgnoreCase))
            {
                found.Add(skill);
            }
        }
        return found;
    }
}
}