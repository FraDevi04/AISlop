// src/AIWrapper.cs
using AISlop;

using LlmTornado;
using LlmTornado.Chat;
using LlmTornado.Chat.Models;
using LlmTornado.Code;
using System.Text;

namespace AISlop
{
    public class AIWrapper
    {
        /// <summary>
        /// Api support: https://github.com/lofcz/LlmTornado
        /// Note: this is a private VPN IP change this to either "localhost" or your LLM server
        /// </summary>
        TornadoApi api = new(new Uri(Config.Settings.ollama_url)); // default Ollama port, API key can be passed in the second argument if needed
        Conversation _conversation = null!;
        readonly int _streamingStates;
        public AIWrapper(string model, int streamingState)
        {
            _conversation = api.Chat.CreateConversation(new ChatRequest()
            {
                Model = model,
                Temperature = 0.2,
                TopP = 0.9,
                FrequencyPenalty = 0.2
            });
            _conversation.AddSystemMessage(GetInstruction("SlopInstruction"));
            _conversation.AddSystemMessage(GetInstruction("WebDev"));
            _streamingStates = streamingState;
        }
        public async Task<string> AskAi(string message, Func<Command, Task<string>> toolExecutor)
        {
            var responseBuilder = new StringBuilder();
            ParserContext context = new("", 0);
            ParserContext.ToolOutputs = "";

            bool streamThought = false;

            // just aesthetic
            if ((_streamingStates & (int)ProcessingState.StreamingToolCalls) != 0)
                Console.WriteLine();

            await _conversation.AppendUserInput(message)
                .StreamResponse(chunk =>
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
                        var toolOutput = toolExecutor(toolCommand).Result;

                        Logging.DisplayToolCallUsage(toolOutput);
                        ParserContext.ToolOutputs += $"{toolOutput}\n";
                    }
                });

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
