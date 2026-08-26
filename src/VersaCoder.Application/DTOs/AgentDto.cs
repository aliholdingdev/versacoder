namespace VersaCoder.Application.DTOs;

public class AgentDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
    public List<string> Tools { get; set; } = new();
}
