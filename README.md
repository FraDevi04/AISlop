# AI Slop

A simple, extensible, and interactive AI agent framework for .NET. This project provides a foundation for building autonomous agents that can reason, use tools, and interact with your local environment to complete complex tasks.

## Features

*   **ReAct-style Agent Logic**: The agent operates in a Reason-Act loop, thinking through a problem, selecting a tool, and observing the result before deciding on the next step.
*   **Extensible Tool System**: Easily add new capabilities by implementing the `ITool` interface. The agent dynamically discovers and loads all available tools at runtime.
*   **Flexible LLM Backend**: Connects to any OpenAI-compatible API endpoint using the [LlmTornado](https://github.com/lofcz/LlmTornado) library. The default configuration is set for local LLMs via Ollama.
*   **Rich Toolset**: Comes with a variety of built-in tools for:
    *   **Web Interaction**: Search the web and scrape text content from web pages.
    *   **File System Operations**: List, read, write, create directories, and even generate PDF files from Markdown.
    *   **Terminal Execution**: Run arbitrary shell commands in a sandboxed `environment` directory.
    *   **User Interaction**: The agent can pause and ask the user for clarification or input.
*   **Transparent Streaming UI**: Watch the agent's `<thought>` process and raw `<tool_call>` streams in real-time for better insight into its decision-making.
*   **Configuration Driven**: Easily configure the model, API endpoint, keys, and UI verbosity via a simple `config.json` file.

## Getting Started

Follow these steps to get the AI Slop agent running on your local machine.

### Prerequisites

*   [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later.
*   An OpenAI-compatible LLM API endpoint. For local hosting, [Ollama](https://ollama.com/) is recommended.
*   An LLM that is good at following instructions and generating structured XML/HTML. A good starting point is `llama3:8b-instruct`. You can pull it with:
    ```sh
    ollama pull llama3:8b-instruct
    ```

### 1. Clone the Repository

```sh
git clone <your-repository-url>
cd AISlop
```

### 2. Configure the Agent

A `config.json` file is used to set up the agent. A default file will be created in a `config` directory on the first run if one doesn't exist.

**`config/config.json`**
```json
{
  "model_name": "llama3:8b-instruct",
  "generate_log": true,
  "display_thought": true,
  "display_toolcall": true,
  "api_url": "http://localhost:11434",
  "api_key": "ollama",
  "search_api_key": "YOUR_GOOGLE_SEARCH_API_KEY"
}
```
> **Important**: Change `api_url` if your LLM provider is not running on the default Ollama address. The `api_key` is typically ignored by Ollama but required by other services.

### 3. Run the Application

Execute the project from your terminal:

```sh
dotnet run
```

The application will start and prompt you for an initial task. For example:

```
Task: Research the key features of the .NET 8 SDK, summarize them in a markdown file, and then create a PDF report from that summary.
```

The agent will then begin its work!

## Configuration

The `config.json` file controls the agent's behavior.

| Setting | Type | Description |
| :--- | :--- | :--- |
| `model_name` | string | The name of the model to use (e.g., `llama3:8b-instruct`, `gpt-4o`). |
| `generate_log` | boolean | If `true`, all console output will be saved to a timestamped log file. |
| `display_thought` | boolean | If `true`, the agent's `<thought>` process will be streamed to the console. |
| `display_toolcall`| boolean | If `true`, the raw XML for the tool calls will be streamed to the console. |
| `api_url` | string | The base URL for your LLM API endpoint (e.g., `http://localhost:11434` for Ollama). |
| `api_key` | string | Your API key for the LLM service. Can be set to any value (e.g., "ollama") for local models. |
| `search_api_key`| string | (Optional) Your API key for services like Google Search if you replace the default web scraper. |

## Available Tools

The agent comes with a suite of tools to interact with its environment.

### File System

*   `createdirectory`: Creates a new directory. (`dirname`)
*   `changedirectory`: Changes the current working directory. (`dirname`)
*   `listdirectory`: Lists all files and subdirectories in the current directory.
*   `writefile`: Creates or overwrites a file with the specified content. (`filename`, `content`)
*   `readfile`: Reads the content of a specified file (supports plain text and `.pdf`). (`filename`)
*   `createpdffile`: Creates a PDF document from Markdown-formatted text. (`filename`, `markdown_content`)

### Web

*   `websearch`: Performs a web search using DuckDuckGo and returns a list of results. (`query`)
*   `gettextfromwebpage`: Scrapes and returns the clean text content from a given URL. (`url`)

### Task Management

*   `askuser`: Pauses execution and asks the user a clarifying question. (`question`)
*   `taskdone`: Informs the user that the task is complete and waits for a new task or an "end" command. (`message`)

### Terminal

*   `executeterminal`: Executes a given command in the system's command line (`cmd.exe` on Windows). **(Use with caution!)** Commands are executed within the agent's CWD. (`command`)

## How It Works

The project follows a simple but powerful agent architecture:

1.  **Input**: The user provides an initial high-level task.
2.  **System Prompt**: A detailed system prompt (loaded from `instructions/*.md`) instructs the model on how to behave, what tools are available, and the required XML-based output format.
3.  **Inference**: The `AIWrapper` sends the user's prompt and conversation history to the LLM via the `LlmTornado` library.
4.  **Parsing**: The `AgentHandler` receives the streaming response. The `XMLParser` class is designed to extract the agent's `<thought>` and `<tool_call>` blocks from the text stream as they arrive.
5.  **Execution**: For each valid tool call parsed, the `AgentHandler` invokes the corresponding `ITool` implementation, passing the arguments and the execution context (which includes the current working directory).
6.  **Observation**: The results (output) from the executed tools are collected.
7.  **Loop**: The tool results are formatted and fed back to the LLM as context for its next step. This Reason-Act loop continues until the agent determines the task is complete and calls the `taskdone` tool.

## Key Dependencies

*   [LlmTornado](https://github.com/lofcz/LlmTornado) - For seamless communication with OpenAI-compatible APIs, including Ollama.
*   [AngleSharp](https://github.com/AngleSharp/AngleSharp) - For robust web scraping and HTML parsing.
*   [QuestPDF](https://www.questpdf.com/) - For easy and fluent PDF document generation from code.
*   [UglyToad.PdfPig](https://github.com/UglyToad/PdfPig) - For extracting text content from PDF files.
