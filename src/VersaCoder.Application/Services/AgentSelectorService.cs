using VersaCoder.Abstractions.Services;

namespace VersaCoder.Application.Services;

public class AgentSelectorService
{
    private readonly IAgentRunner _agentRunner;
    private readonly IToolExecutor _toolExecutor;

    public AgentSelectorService(
        IAgentRunner agentRunner,
        IToolExecutor toolExecutor)
    {
        _agentRunner = agentRunner;
        _toolExecutor = toolExecutor;
    }

    public string SelectAgent(string prompt, Dictionary<string, object> context)
    {
        var lowerPrompt = prompt.ToLowerInvariant();

        if (lowerPrompt.Contains("plan") || lowerPrompt.Contains("mimari") || lowerPrompt.Contains("task"))
            return "plan";

        if (lowerPrompt.Contains("analiz") || lowerPrompt.Contains("tara") || lowerPrompt.Contains("bul"))
            return "explore";

        if (lowerPrompt.Contains("özet") || lowerPrompt.Contains("doküman"))
            return "summary";

        if (lowerPrompt.Contains("başlık") || lowerPrompt.Contains("isimlendir"))
            return "title";

        return "build";
    }
}
