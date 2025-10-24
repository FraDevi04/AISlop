// src/AgentHandler.cs
namespace AISlop
{
    public class AgentHandler
    {
        private readonly AIWrapper _agent;
        private string _cwd = "environment";
        private bool _agentRunning = true;
        private bool _taskDone = false;
        private readonly Dictionary<string, ITool> _tools;

        /// <summary>
        /// Initializes the Tools, Agent, and a ToolHandler for this instance
        /// </summary>
        /// <param name="modelName">AI model name</param>
        public AgentHandler(IEnumerable<ITool> availableTools, AIWrapper wrapper)
        {
            _agent = wrapper;
            _tools = availableTools.ToDictionary(t => t.Name.ToLower());
        }
        /// <summary>
        /// Main function of the agent. Handles the recursion
        /// </summary>
        /// <param name="initialTask">Task string, what the agent should do</param>
        /// <exception cref="ArgumentNullException">Invalid task was given</exception>
        public async Task RunAsync(string initialTask)
        {
            if (string.IsNullOrWhiteSpace(initialTask))
                throw new ArgumentNullException("Task was an empty string!");

            Logging.DisplayAgentThought(ConsoleColor.Green);
            var agentResponse = await _agent.AskAi($"{initialTask}\nCurrent cwd: \"{_cwd}\"", ExecuteTool);

            while(_agentRunning)
                agentResponse = await HandleAgentResponse(agentResponse);
        }
        /// <summary>
        /// Handles the agents response and task phases
        /// </summary>
        /// <param name="rawResponse">Agents response (raw response)</param>
        /// <returns>API Responses</returns>
        private async Task<string> HandleAgentResponse(string rawResponse)
        {
            var toolCallOutputs = ParserContext.ToolOutputs;
            if (_taskDone)
                return await HandleTaskCompletion(toolCallOutputs.GetValueOrDefault("TaskDone"));
            else
                return await ContinueAgent(toolCallOutputs);
        }
        /// <summary>
        /// Executes tools in order
        /// </summary>
        /// <param name="toolcalls">Tools to execute</param>
        /// <returns>Tool outputs</returns>
        private async Task<string> ExecuteTool(Command singleCall)
        {
            string currentToolName = "";
            try
            {
                currentToolName = singleCall.Tool;
                if (_tools.TryGetValue(singleCall.Tool.ToLower(), out var tool))
                {
                    var context = new ToolExecutionContext { CurrentWorkingDirectory = _cwd };
                    string result = await tool.ExecuteAsync(singleCall.Args, context);
                    _cwd = context.CurrentWorkingDirectory;

                    if (currentToolName.ToLower() == "taskdone")
                        _taskDone = true;
                    else 
                        return ($"{singleCall.Tool} output: {result}");
                }
                else
                    return ($"{singleCall.Tool} error: Tool not found.");
            }
            catch (Exception ex)
            {
                return ($"An exception occurred during {currentToolName} execution: {ex.Message}");
            }
            return ($"Task is marked as completed!");
        }
        /// <summary>
        /// Task ended handle. `end` ends the current chat
        /// New prompt will launch a follow up to the task it was doing before.
        /// </summary>
        /// <param name="completeMessage">Agent completition message</param>
        /// <returns>Agent response</returns>
        /// <exception cref="ArgumentNullException">No task was given</exception>
        private async Task<string> HandleTaskCompletion(string completeMessage)
        {
            _agentRunning = false;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("New task: (type \"end\" to end the process)");
            string newTask = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(newTask))
                throw new ArgumentNullException("Task was an empty string!");
            if (newTask.ToLower() == "end")
                return completeMessage;

            Console.WriteLine();
            _agentRunning = true;
            _taskDone = false;
            Logging.DisplayAgentThought(ConsoleColor.Green);
            return await _agent.AskAi($"User followup question/task: {newTask}\nCurrent cwd: \"{_cwd}\"", ExecuteTool);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="toolOutput"></param>
        /// <returns></returns>
        private async Task<string> ContinueAgent(Dictionary<string, string> toolOutput)
        {
            Logging.DisplayAgentThought(ConsoleColor.Green);
            toolOutput.Add("cwd", $"Current cwd: {_cwd}");
            return await _agent.AskAiLoopCallback(
                toolOutput,
                ExecuteTool
            );
        }
    }
}
