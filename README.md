# AI Slop

A simple, extensible, and interactive AI agent framework for .NET, powered by local LLMs via Ollama. This project provides a foundation for building autonomous agents that can reason, use tools, and interact with your local environment to complete tasks.

 
*(Image placeholder: A GIF or screenshot showing the agent running, displaying its thoughts, executing a tool like `websearch`, and then writing the results to a file would be perfect here.)*

## Features

*   **ReAct-style Agent Logic**: The agent operates in a Reason-Act loop, thinking through a problem, selecting a tool, and observing the result before deciding on the next step.
*   **Tool-Enabled**: Easily extensible with a variety of built-in tools for:
    *   **Web Browsing**: Search the web and scrape text from web pages.
    *   **File System Operations**: List, read, write, create directories, and even generate PDF files.
    *   **Terminal Execution**: Run arbitrary shell commands in a dedicated workspace.
    *   **User Interaction**: The agent can ask the user for clarification or additional information.
*   **Powered by Ollama**: Connects to any Ollama-hosted model, giving you full control over the AI brain.
*   **Streaming UI**: Watch the agent's `thoughts` and `tool_calls` stream in real-time for better transparency.
*   **Configurable**: Easily configure the model, Ollama URL, and UI verbosity via a simple `config.json` file.

## Getting Started

Follow these steps to get the AI Slop agent running on your local machine.

### Prerequisites

*   [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later.
*   [Ollama](https://ollama.com/) installed and running.
*   An Ollama model that supports JSON mode and function/tool calling. The default in the config is `qwen3:4b-instruct-2507-q8_0`. You can pull it with:
    ```sh
    ollama pull qwen3:4b-instruct-2507-q8_0
    ```

### 1. Clone the Repository

```sh
git clone <your-repository-url>
cd AISlop
```

### 2. Configure the Agent

A configuration file is used to set up the agent. A default `config.json` will be created on the first run.

1.  Navigate to the `config` directory (it will be created if it doesn't exist).
2.  Open or create `config.json` and modify it to your needs.

**`config/config.json`**
```json
{
  "model_name": "qwen3:4b-instruct-2507-q8_0",
  "generate_log": false,
  "display_thought": true,
  "display_toolcall": true,
  "ollama_url": "http://localhost:11434"
}
```
> **Important**: Change `ollama_url` if your Ollama instance is not running on the default localhost address.

### 3. Run the Application

Execute the project from your terminal:

```sh
dotnet run
```

The application will start and prompt you for an initial task. For example:

```
Task: Research the current price of Bitcoin, summarize your findings in a text file, and then create a PDF report from that summary.
```

The agent will then begin its work!

## Configuration

The `config.json` file controls the agent's behavior.

| Setting            | Type    | Description                                                                                               |
| ------------------ | ------- | --------------------------------------------------------------------------------------------------------- |
| `model_name`       | string  | The name of the Ollama model to use (e.g., `llama3:8b`, `qwen3:4b-instruct-2507-q8_0`).                     |
| `generate_log`     | boolean | If `true`, all console output will be saved to a timestamped log file in the root directory.              |
| `display_thought`  | boolean | If `true`, the agent's "thought" process will be streamed to the console in real-time.                      |
| `display_toolcall` | boolean | If `true`, the raw JSON for the tool calls will be streamed to the console.                               |
| `ollama_url`       | string  | The base URL for your Ollama API endpoint.                                                                |

## Available Tools

The agent comes with a suite of tools to interact with its environment.

### File System

*   `createdirectory`: Creates a new directory.
*   `changedirectory`: Changes the current working directory.
*   `listdirectory`: Lists all files and subdirectories in the current directory.
*   `writefile`: Creates or overwrites a file with the specified content.
*   `readfile`: Reads the content of a specified file (supports `.txt` and `.pdf`).
*   `createpdffile`: Creates a PDF document from Markdown-formatted text.

### Web

*   `websearch`: Performs a web search using DuckDuckGo and returns a list of results.
*   `gettextfromwebpage`: Scrapes and returns the clean text content from a given URL.

### Task Management

*   `askuser`: Pauses execution and asks the user a clarifying question.
*   `taskdone`: Informs the user that the task is complete and waits for a new task or an "end" command.

### Terminal

*   `executeterminal`: Executes a given command in the system's command line (`cmd.exe`). **(Use with caution!)**

## How It Works

The project follows a simple but powerful agent architecture:

1.  **Input**: The user provides an initial high-level task.
2.  **System Prompt**: A detailed system prompt (from `instructions/SlopInstruction.md`) instructs the model on how to behave, what tools are available, and the required JSON output format.
3.  **Inference**: The `AIWrapper` sends the user's prompt to the Ollama LLM.
4.  **Parsing**: The `AgentHandler` receives the raw LLM response. The `Parser` class extracts the agent's `thought` and a list of `tool_calls` from the JSON response.
5.  **Execution**: The `AgentHandler` iterates through the requested `tool_calls` and executes the corresponding `ITool` implementation.
6.  **Observation**: The results (output) from the executed tools are collected.
7.  **Loop**: The tool results are fed back to the LLM as context for its next step. This loop continues until the agent determines the task is complete and calls the `taskdone` tool.

## Key Dependencies

*   [LlmTornado](https://github.com/lofcz/LlmTornado) - For seamless communication with the Ollama API.
*   [AngleSharp](https://github.com/AngleSharp/AngleSharp) - For robust web scraping and HTML parsing.
*   [QuestPDF](https://www.questpdf.com/) - For easy and fluent PDF document generation.
*   [UglyToad.PdfPig](https://github.com/UglyToad/PdfPig) - For extracting text content from PDF files.