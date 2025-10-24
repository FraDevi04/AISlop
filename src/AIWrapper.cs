// src/AIWrapper.cs
using LlmTornado;
using LlmTornado.Chat;
using System.Text;

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
            //_conversation.AddSystemMessage(GetInstruction("TailwindInstallation"));
            _conversation.AddSystemMessage($"Current date: {DateTime.Today.ToString("d")}");
            _streamingStates = streamingState;
        }
        public async Task<string> AskAi(string message, Func<Command, Task<string>> toolExecutor)
        {
            var responseBuilder = new StringBuilder();
            ParserContext context = new("", 0);
            ParserContext.ToolOutputs = new();

            bool streamThought = false;
            bool toolRunning = false;

            // just aesthetic
            if ((_streamingStates & (int)ProcessingState.StreamingToolCalls) != 0)
                Console.WriteLine();

            await _conversation.AppendUserInput(message)
                .StreamResponse(async chunk =>
                {
                    responseBuilder.Append(chunk);
                    string currentFullResponse = responseBuilder.ToString();

                    bool stopStream = currentFullResponse.Count(a => a == '<') > 1;
                    if (stopStream)
                        streamThought = false;
                    else if (currentFullResponse.Contains("<thought>") && !chunk.Contains(">"))
                        streamThought = true;

                    if (!streamThought && (_streamingStates & (int)ProcessingState.StreamingToolCalls) != 0)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write(chunk);
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }

                    if (streamThought)
                        Console.Write(chunk);

                    context.Buffer = currentFullResponse;
                    var toolCommand = XMLParser.ParseCurrent(context);
                    if (toolCommand != null)
                    {
                        toolRunning = true;
                        var toolOutput = await toolExecutor(toolCommand);

                        Logging.DisplayToolCallUsage(toolOutput);
                        if (ParserContext.ToolOutputs.ContainsKey(toolCommand.Tool))
                        {
                            ParserContext.ToolOutputs[toolCommand.Tool] += $"\n{toolOutput}";
                        }
                        else
                        {
                            ParserContext.ToolOutputs.Add(toolCommand.Tool, toolOutput);
                        }
                        toolRunning = false;
                    }
                });

            while (toolRunning)
                Thread.Sleep(1);

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine();
            return responseBuilder.ToString();
        }

        public async Task<string> AskAiLoopCallback(Dictionary<string, string> message, Func<Command, Task<string>> toolExecutor)
        {
            var responseBuilder = new StringBuilder();
            ParserContext context = new("", 0);
            ParserContext.ToolOutputs = new();

            bool streamThought = false;
            bool toolRunning = false;

            // just aesthetic
            if ((_streamingStates & (int)ProcessingState.StreamingToolCalls) != 0)
                Console.WriteLine();

            foreach (var item in message)
            {
                Console.WriteLine($"{item.Key} {item.Value}");
                _conversation.AppendFunctionMessage(item.Key, item.Value);
            }

            await _conversation.AppendUserInput(message["cwd"])
                .StreamResponse(async chunk =>
                {
                    responseBuilder.Append(chunk);
                    string currentFullResponse = responseBuilder.ToString();

                    bool stopStream = currentFullResponse.Count(a => a == '<') > 1;
                    if (stopStream)
                        streamThought = false;
                    else if (currentFullResponse.Contains("<thought>") && !chunk.Contains(">"))
                        streamThought = true;

                    if (!streamThought && (_streamingStates & (int)ProcessingState.StreamingToolCalls) != 0)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write(chunk);
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }

                    if (streamThought)
                        Console.Write(chunk);

                    context.Buffer = currentFullResponse;
                    var toolCommand = XMLParser.ParseCurrent(context);
                    if (toolCommand != null)
                    {
                        toolRunning = true;
                        var toolOutput = await toolExecutor(toolCommand);

                        Logging.DisplayToolCallUsage(toolOutput);
                        if (ParserContext.ToolOutputs.ContainsKey(toolCommand.Tool))
                        {
                            ParserContext.ToolOutputs[toolCommand.Tool] += $"\n{toolOutput}";
                        }
                        else
                        {
                            ParserContext.ToolOutputs.Add(toolCommand.Tool, toolOutput);
                        }
                        toolRunning = false;
                    }
                });

            while (toolRunning)
                Thread.Sleep(1);

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine();
            return responseBuilder.ToString();        
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
}
