// src/tools/googlecalendar/ListCalendarEvents.cs
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;

namespace AISlop;

public class ListCalendarEvents : ITool
{
    public string Name => "list_calendar_events";

    public async Task<string> ExecuteAsync(Dictionary<string, string> args, ToolExecutionContext context)
    {
        try
        {
            var service = await GoogleCalendarHelper.GetServiceAsync();
            
            string? calendarId = args.GetValueOrDefault("calendar_id") ?? "primary";
            string? timeMinStr = args.GetValueOrDefault("time_min");
            string? timeMaxStr = args.GetValueOrDefault("time_max");
            int? maxResults = args.TryGetValue("max_results", out var maxResultsStr) && int.TryParse(maxResultsStr, out var max) ? max : 10;

            var request = service.Events.List(calendarId);
            request.MaxResults = maxResults;
            request.SingleEvents = true;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            if (!string.IsNullOrEmpty(timeMinStr))
            {
                if (DateTime.TryParse(timeMinStr, out var timeMin))
                {
                    request.TimeMinDateTimeOffset = timeMin;
                }
            }
            else
            {
                request.TimeMinDateTimeOffset = DateTimeOffset.Now;
            }

            if (!string.IsNullOrEmpty(timeMaxStr) && DateTime.TryParse(timeMaxStr, out var timeMax))
            {
                request.TimeMaxDateTimeOffset = timeMax;
            }

            var events = await request.ExecuteAsync();

            if (events.Items == null || events.Items.Count == 0)
            {
                return "No events found in the specified time range.";
            }

            var result = new System.Text.StringBuilder();
            result.AppendLine($"Found {events.Items.Count} event(s):\n");

            foreach (var eventItem in events.Items)
            {
                result.AppendLine($"Event: {eventItem.Summary ?? "(No title)"}");
                
                if (eventItem.Start != null)
                {
                    if (eventItem.Start.DateTimeDateTimeOffset != null)
                    {
                        result.AppendLine($"  Start: {eventItem.Start.DateTimeDateTimeOffset:yyyy-MM-dd HH:mm}");
                    }
                    else if (eventItem.Start.Date != null)
                    {
                        result.AppendLine($"  Start: {eventItem.Start.Date} (All day)");
                    }
                }

                if (eventItem.End != null)
                {
                    if (eventItem.End.DateTimeDateTimeOffset != null)
                    {
                        result.AppendLine($"  End: {eventItem.End.DateTimeDateTimeOffset:yyyy-MM-dd HH:mm}");
                    }
                    else if (eventItem.End.Date != null)
                    {
                        result.AppendLine($"  End: {eventItem.End.Date} (All day)");
                    }
                }

                if (!string.IsNullOrEmpty(eventItem.Description))
                {
                    result.AppendLine($"  Description: {eventItem.Description}");
                }

                if (!string.IsNullOrEmpty(eventItem.Location))
                {
                    result.AppendLine($"  Location: {eventItem.Location}");
                }

                if (!string.IsNullOrEmpty(eventItem.HtmlLink))
                {
                    result.AppendLine($"  Link: {eventItem.HtmlLink}");
                }
                result.AppendLine($"  ID: {eventItem.Id}");
                result.AppendLine();
            }

            return result.ToString();
        }
        catch (Exception ex)
        {
            return $"Error listing calendar events: {ex.Message}";
        }
    }
}
