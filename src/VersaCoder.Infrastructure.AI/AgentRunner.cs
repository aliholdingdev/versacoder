using System.Diagnostics;
using Microsoft.Extensions.Logging;
using VersaCoder.Abstractions.Providers;
using VersaCoder.Abstractions.Services;
using VersaCoder.Domain.Constants;

namespace VersaCoder.Infrastructure.AI;

public class AgentRunner : IAgentRunner
{
    private readonly ProviderRouter _router;
    private readonly IToolExecutor _toolExecutor;
    private readonly IContextManager _contextManager;
    private readonly ILogger<AgentRunner> _logger;

    public AgentRunner(
        ProviderRouter router,
        IToolExecutor toolExecutor,
        IContextManager contextManager,
        ILogger<AgentRunner> logger)
    {
        _router = router;
        _toolExecutor = toolExecutor;
        _contextManager = contextManager;
        _logger = logger;
    }

    public async Task<AgentResponse> RunAsync(AgentRequest request, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var agentName = request.AgentName ?? SelectAgent(request.Prompt);

        _logger.LogInformation("Running agent {Agent} for prompt: {Prompt}",
            agentName, request.Prompt[..Math.Min(100, request.Prompt.Length)]);

        try
        {
            // Assemble context
            var context = await _contextManager.AssembleAsync(
                Domain.Enums.AgentRole.BUILD,
                request.SessionId,
                cancellationToken);

            // Build messages
            var messages = new List<LLMMessage>
            {
                new() { Role = "user", Content = request.Prompt }
            };

            // Get available tools
            var tools = await _toolExecutor.GetAvailableToolsAsync(agentName, cancellationToken);

            // Route to LLM provider
            var llmRequest = new LLMRequest
            {
                SystemPrompt = BuildSystemPrompt(agentName),
                Messages = messages,
                Tools = tools.Select(t => new LLMTool
                {
                    Name = t.Name,
                    Description = t.Description,
                    Parameters = t.Schema
                }).ToList()
            };

            var response = await _router.RouteAsync(llmRequest, cancellationToken: cancellationToken);
            stopwatch.Stop();

            // Handle tool calls if any
            var toolResults = new List<ToolCallResult>();
            if (response.ToolCalls?.Any() == true)
            {
                foreach (var toolCall in response.ToolCalls)
                {
                    var toolResult = await _toolExecutor.ExecuteAsync(new ToolCallRequest
                    {
                        ToolName = toolCall.Name,
                        Parameters = toolCall.Arguments,
                        AgentName = agentName
                    }, cancellationToken);

                    toolResults.Add(new ToolCallResult
                    {
                        ToolName = toolCall.Name,
                        Parameters = toolCall.Arguments,
                        Result = toolResult.Output,
                        Success = toolResult.Success
                    });
                }
            }

            return new AgentResponse
            {
                Content = response.Content,
                AgentName = agentName,
                HasToolCalls = toolResults.Any(),
                ToolCalls = toolResults,
                Duration = stopwatch.Elapsed
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error running agent {Agent}", agentName);
            stopwatch.Stop();
            return new AgentResponse
            {
                Content = $"Error: {ex.Message}",
                AgentName = agentName,
                Duration = stopwatch.Elapsed
            };
        }
    }

    public async IAsyncEnumerable<AgentStreamChunk> RunStreamingAsync(
        AgentRequest request,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var agentName = request.AgentName ?? SelectAgent(request.Prompt);

        var messages = new List<LLMMessage>
        {
            new() { Role = "user", Content = request.Prompt }
        };

        var llmRequest = new LLMRequest
        {
            SystemPrompt = BuildSystemPrompt(agentName),
            Messages = messages
        };

        var provider = _router.GetProvider();
        await foreach (var chunk in provider.SendStreamingMessageAsync(llmRequest, cancellationToken))
        {
            yield return new AgentStreamChunk
            {
                Content = chunk.Content,
                IsComplete = chunk.IsComplete
            };
        }
    }

    private string SelectAgent(string prompt)
    {
        var lower = prompt.ToLowerInvariant();

        if (lower.Contains("plan") || lower.Contains("mimari") || lower.Contains("task"))
            return AgentNames.PLAN;

        if (lower.Contains("analiz") || lower.Contains("tara") || lower.Contains("bul"))
            return AgentNames.EXPLORE;

        if (lower.Contains("özet") || lower.Contains("doküman"))
            return AgentNames.SUMMARY;

        if (lower.Contains("başlık") || lower.Contains("isimlendir"))
            return AgentNames.TITLE;

        return AgentNames.BUILD;
    }

    private string BuildSystemPrompt(string agentName)
    {
        return agentName switch
        {
            AgentNames.BUILD => "You are a Build Agent. Write clean, production-ready C# code following Clean Architecture, SOLID principles, and DDD patterns.",
            AgentNames.PLAN => "You are a Plan Agent. Analyze requirements and create detailed technical plans with task breakdown.",
            AgentNames.EXPLORE => "You are an Explore Agent. Analyze codebases, find patterns, and provide insights.",
            AgentNames.GENERAL => "You are a General Agent. Handle multi-domain tasks efficiently.",
            AgentNames.SUMMARY => "You are a Summary Agent. Create concise, accurate summaries and documentation.",
            AgentNames.TITLE => "You are a Title Agent. Create clear, descriptive names following conventions.",
            _ => "You are a VersaCoder AI assistant."
        };
    }
}
