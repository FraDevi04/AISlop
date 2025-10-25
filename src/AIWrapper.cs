// src/AIWrapper.cs
using LlmTornado;
using LlmTornado.Chat;
using LlmTornado.Code;
using System.Text;
using System.Text.RegularExpressions;

namespace AISlop
{
    public class AIWrapper
    {
        /// <summary>
        /// Api support: https://github.com/lofcz/LlmTornado
        /// </summary>
        TornadoApi api = new(new Uri(Config.Settings.api_url), Config.Settings.api_key);
        Conversation _conversation = null!;
        readonly int _streamingStates;
        public AIWrapper(string model, int streamingState)
        {
            _conversation = api.Chat.CreateConversation(new ChatRequest()
            {
                Model = model,
            });
            
            _conversation.AddSystemMessage(GetInstruction("SlopInstruction"));
            _conversation.AddSystemMessage($"Current date: {DateTime.Today.ToString("d")}");
            _streamingStates = streamingState;
        }
        private async Task<string> ProcessStreamedResponseAsync(Conversation turn, Func<Command, Task<string>> toolExecutor)
        {
            ParserContext.ToolOutputs = new Dictionary<string, string>();
            turn = TruncateMessages();

            // Aesthetic: add a new line if we are streaming tool calls
            if ((_streamingStates & (int)ProcessingState.StreamingToolCalls) != 0)
                Console.WriteLine();

            using var ctx = new AIStreamContext(toolExecutor);
            await foreach (string res in turn.StreamResponseEnumerable(token: ctx.cancellationToken.Token))
            {
                await StreamHandler(res, ctx);
                if (ctx.cancellationToken.IsCancellationRequested) // cancellation token doesn't work
                    break;
            }

            // Wait for the triggered tool task (if any) to complete before returning.
            while (ctx.toolRunning)
                await Task.Delay(1);

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine();
            _conversation.AppendAssistantMessage(ctx.responseBuilder.ToString());
            return ctx.responseBuilder.ToString();
        }

        public async Task StreamHandler(string chunk, AIStreamContext ctx)
        {
            ctx.responseBuilder.Append(chunk);
            string currentFullResponse = ctx.responseBuilder.ToString();

            // Logic to decide whether to stream the model's "thought" process to the console
            bool stopStream = currentFullResponse.Count(c => c == '<') > 1;
            if (stopStream)
                ctx.streamThought = false;
            else if (currentFullResponse.Contains("<thought>") && !chunk.Contains(">"))
                ctx.streamThought = true;

            // Stream the response with different colors for thoughts vs. final output
            if (ctx.streamThought)
            {
                Console.Write(chunk);
            }
            else if ((_streamingStates & (int)ProcessingState.StreamingToolCalls) != 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write(chunk);
                Console.ForegroundColor = ConsoleColor.Gray;
            }

            ctx.parserContext.Buffer = currentFullResponse;
            var toolCommand = XMLParser.ParseCurrent(ctx.parserContext);
            if (toolCommand != null)
            {
                ctx.toolRunning = true;
                var toolOutput = await ctx.toolExecutor(toolCommand);
                await ctx.cancellationToken.CancelAsync();

                Logging.DisplayToolCallUsage(toolOutput);
                if (!ParserContext.ToolOutputs!.TryAdd(toolCommand.Tool, toolOutput))
                {
                    ParserContext.ToolOutputs[toolCommand.Tool] += $"\n{toolOutput}";
                }

                ctx.toolRunning = false;
            }
        }

        /// <summary>
        /// Sends a single user message to the AI and processes the streamed response for tool calls.
        /// </summary>
        public async Task<string> AskAi(string message, Func<Command, Task<string>> toolExecutor)
        {
            var turn = _conversation.AppendUserInput(message);
            return await ProcessStreamedResponseAsync(turn, toolExecutor);
        }

        /// <summary>
        /// Appends previous tool outputs to the conversation, sends a new user prompt,
        /// and processes the streamed response for further tool calls.
        /// </summary>
        /// <param name="toolOutputsAndPrompt">
        /// A dictionary containing tool outputs to be added to the conversation history.
        /// It must also contain the key "cwd" which will be used as the next user prompt.
        /// </param>
        public async Task<string> AskAi(Dictionary<string, string> toolOutputsAndPrompt, Func<Command, Task<string>> toolExecutor)
        {
            foreach (var item in toolOutputsAndPrompt)
            {
                if (item.Key.Equals("cwd", StringComparison.OrdinalIgnoreCase)) continue;

                _conversation.AppendUserInputWithName(item.Key, $"Tooloutput: {item.Value}");
            }

            var turn = _conversation.AppendUserInput(toolOutputsAndPrompt["cwd"]);
            return await ProcessStreamedResponseAsync(turn, toolExecutor);
        }

        public Conversation TruncateMessages()
        {
            var truncatedConversation = api.Chat.CreateConversation(new ChatRequest()
            {
                Model = _conversation.Model
            });
            truncatedConversation.AppendUserInput(_conversation.Messages.First(user => user.Role == ChatMessageRoles.User).Content);
            const string pattern = @"<thought>(.*?)<\/thought>";
            StringBuilder sb = new();
            foreach (var item in _conversation.Messages)
            {
                switch (item.Role)
                {
                    case ChatMessageRoles.System:
                        truncatedConversation.AppendSystemMessage(item.Content);
                        break;

                    case ChatMessageRoles.User:
                        {
                            if (!string.IsNullOrWhiteSpace(item.Name))
                                truncatedConversation.AppendUserInputWithName(item.Name, item.Content);
                        }
                        break;

                    case ChatMessageRoles.Assistant:
                        {
                            //break; // truncate assistant message
                            var match = Regex.Match(item.Content, pattern, RegexOptions.Singleline).Value;
                            truncatedConversation.AppendAssistantMessage(match.Replace("<thought>", "").Replace("</thought>", ""));
                        }
                        break;
                }
            }
            truncatedConversation.AppendUserInput(_conversation.Messages.Last(a => a.Content.Contains("Current cwd")).Content);
            //Console.WriteLine("\n-----------------CURRENT MESSAGE HISTORY-----------------\n");
            //foreach (var item in truncatedConversation.Messages)
            //{
            //    if (item.Role != ChatMessageRoles.System)
            //    {
            //        Console.WriteLine($"{item.Role} {item.Name} {item.Content}\n");
            //    }
            //}
            //Console.WriteLine("-----------------END-----------------");
            return truncatedConversation;
        }

        private string GetInstruction(string instructName)
        {
            string solutionDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
            string instructFolder = Path.Combine(solutionDir, "instructions");

            if (!Directory.Exists(instructFolder))
                throw new DirectoryNotFoundException("Instructions folder not found.");

            return File.ReadAllText(Path.Combine(instructFolder, $"{instructName}.md"));
        }
    }
    public class AIStreamContext : IDisposable
    {
        public StringBuilder responseBuilder;
        public ParserContext parserContext;
        public CancellationTokenSource cancellationToken;
        public bool streamThought;
        public bool toolRunning;
        public Func<Command, Task<string>> toolExecutor;

        internal AIStreamContext(Func<Command, Task<string>> toolExecutor)
        {
            this.responseBuilder = new StringBuilder();
            this.parserContext = new ParserContext("", 0);
            this.cancellationToken = new CancellationTokenSource();
            this.streamThought = false;
            this.toolRunning = false;
            this.toolExecutor = toolExecutor;
        }

        public void Dispose()
        {
            responseBuilder.Clear();
            cancellationToken.Dispose();
        }
    }

}
