// src/tools/ToolExecutionContext.cs
namespace AISlop
{
    /// <summary>
    /// TODO: Add a queue for tools that shouldn't run async (ASKUSER FOR EXAMPLE???)
    /// </summary>
    public class ToolExecutionContext 
    {
        public string CurrentWorkingDirectory { get; set; }

        public override string ToString()
        {
            return CurrentWorkingDirectory;
        }  
    }
}
