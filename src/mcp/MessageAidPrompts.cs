using System.ComponentModel;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Server;

[McpServerPromptType]
public static class MessageAidPrompts
{
    [McpServerPrompt, Description("Creates a prompt to identify empty queues")]
    public static ChatMessage EmptyQueues() =>
        new(ChatRole.User, "Please list all queues that are empty");
}