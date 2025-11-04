// src/tools/googlecalendar/DeleteCalendarEvent.cs

namespace AISlop;

public class DeleteCalendarEvent : ITool
{
    public string Name => "delete_calendar_event";

    public async Task<string> ExecuteAsync(Dictionary<string, string> args, ToolExecutionContext context)
    {
        try
        {
            var service = await GoogleCalendarHelper.GetServiceAsync();

            string? calendarId = args.GetValueOrDefault("calendar_id") ?? "primary";
            string? eventId = args.GetValueOrDefault("event_id");

            if (string.IsNullOrEmpty(eventId))
            {
                return "Error: 'event_id' parameter is required.";
            }

            await service.Events.Delete(calendarId, eventId).ExecuteAsync();

            return $"Event {eventId} deleted successfully.";
        }
        catch (Exception ex)
        {
            return $"Error deleting calendar event: {ex.Message}";
        }
    }
}
