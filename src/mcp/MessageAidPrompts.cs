using System.ComponentModel;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Server;

[McpServerPromptType]
public static class MessageAidPrompts
{
    [McpServerPrompt, Description("Creates a prompt to summarize the provided message.")]
    public static ChatMessage Summarize([Description("The content to summarize")] string content) =>
        new(ChatRole.User, $"Please summarize this content into a single sentence: {content}");
}