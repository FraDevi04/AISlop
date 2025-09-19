![Status](https://img.shields.io/badge/status-WIP-orange)

This branch contains a proof-of-concept and implementation plan for a new AI tool-calling standard for this project. We are moving away from a JSON-based format to a custom XML/HTML-like tag system designed for superior streaming performance and instant feedback.

## 1. The Problem with JSON Streaming

The current system, where the AI model generates a JSON object containing a list of `tool_calls`, presents a significant challenge for real-time applications:

-   **All-or-Nothing Parsing:** A standard JSON parser can only validate and process the structure once the *entire* document is complete and syntactically valid.
-   **Complex Stream Parsing:** To process tools as they arrive, we must implement complex and fragile logic to find balanced curly braces `{}`, which is prone to errors with nested data.
-   **Delayed Feedback:** Because of the parsing challenges, we cannot execute a tool until its full JSON object, and often the entire parent array, has been received. This creates a noticeable delay between the AI generating a tool call and the user seeing its result.

## 2. The Proposed Solution: Tag-Based Tool Calls

We are replacing the JSON format with a simple, human-readable, and easily parsable tag-based system.

### New Syntax

The format is defined as follows:

```xml
<toolcall(ToolName)>
  <argumentName>Argument Value</argumentName>
  <anotherArgument>Another Value</anotherArgument>
</toolcall(ToolName)>
```

-   The outer tag `toolcall(ToolName)` defines the tool to be executed.
-   Each argument is a simple `<key>value</key>` pair.
-   The parser can identify a complete tool call as soon as it receives the matching closing tag `</toolcall(ToolName)>`.

### Key Advantages

-   ✅ **True, Simple Streaming:** We no longer need to count braces or implement a complex JSON parser. We simply buffer the stream and look for a complete `<toolcall(...)>...</toolcall(...)>` block.
-   🚀 **Instant Feedback:** As soon as a closing tag is received, the tool can be parsed and executed immediately, while the AI continues to generate the rest of its response.
-   🧠 **Improved Readability:** The format is clean and easy for developers to read and debug.
-   🔧 **Simplified Implementation:** Parsing can be handled with simple string operations or a single, effective Regular Expression.

## 3. Implementation Plan & TODO

This branch serves as the working directory for this migration. The following tasks must be completed to make this the new standard.

-   [x] **Initial Proof-of-Concept:** A basic parser using Regex has been implemented in the `StreamResponseWithCallbackAsync` callback to prove viability.
-   [ ] **Create `ToolCallStreamParser` Class:** Refactor the PoC logic into a dedicated, reusable, and stateful class responsible for parsing the incoming stream and invoking tool handlers.
-   [ ] **Update All Tool Handlers:** Systematically replace all existing JSON-based tool call handlers with the new tag-based parser.
-   [ ] **Update AI System Prompts:** This is a critical step. All prompts that instruct the AI to use tools must be updated to request the new `<toolcall>` format instead of JSON.
-   [ ] **Robust Error Handling:** The parser must gracefully handle malformed tags, missing arguments, or incomplete streams.
-   [ ] **Add Comprehensive Unit Tests:** Create a suite of tests for the `ToolCallStreamParser` covering valid tags, nested content, and various failure modes.
-   [ ] **Update Project Documentation:** All developer documentation must be updated to reflect this new standard for AI tool integration.

## 4. Example Migration

### Before (Old `main` branch)

The AI would generate a block of JSON, which would be parsed at the end.

```diff
- {
-  "thought": "I need to write a file.",
-  "tool_calls": [
-   {
-    "tool": "WriteFile",
-    "args": { "filename": "README.md", "content": "This is a new project." }
-   }
-  ]
- }
```

### After (This branch's standard)

The AI generates a stream of text and tags. The `WriteFile` tool is executed the moment its closing tag is processed.

```diff
+ I need to write a file.
+ <toolcall(WriteFile)>
+   <filename>README.md</filename>
+   <content>This is a new project.</content>
+ </toolcall(WriteFile)>
+ OK, the file has been written successfully.
```
