// src/tools/googlecalendar/UpdateCalendarEvent.cs
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using System;

namespace AISlop;

public class UpdateCalendarEvent : ITool
{
    public string Name => "update_calendar_event";

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

            // Get existing event
            var existingEvent = await service.Events.Get(calendarId, eventId).ExecuteAsync();

            // Update fields if provided
            if (args.TryGetValue("summary", out var summary) && !string.IsNullOrEmpty(summary))
            {
                existingEvent.Summary = summary;
            }

            if (args.TryGetValue("description", out var description))
            {
                existingEvent.Description = description;
            }

            if (args.TryGetValue("location", out var location))
            {
                existingEvent.Location = location;
            }

            if (args.TryGetValue("start_datetime", out var startDateTime) && !string.IsNullOrEmpty(startDateTime))
            {
                if (DateTime.TryParse(startDateTime, out var start))
                {
                    var startOffset = new DateTimeOffset(start, TimeZoneInfo.Local.GetUtcOffset(start));
                    existingEvent.Start = new EventDateTime
                    {
                        DateTimeDateTimeOffset = startOffset,
                        TimeZone = TimeZoneInfo.Local.Id
                    };
                }
            }

            if (args.TryGetValue("end_datetime", out var endDateTime) && !string.IsNullOrEmpty(endDateTime))
            {
                if (DateTime.TryParse(endDateTime, out var end))
                {
                    var endOffset = new DateTimeOffset(end, TimeZoneInfo.Local.GetUtcOffset(end));
                    existingEvent.End = new EventDateTime
                    {
                        DateTimeDateTimeOffset = endOffset,
                        TimeZone = TimeZoneInfo.Local.Id
                    };
                }
            }

            // Update the event
            var updatedEvent = await service.Events.Update(existingEvent, calendarId, eventId).ExecuteAsync();

            return $"Event updated successfully:\n" +
                   $"  Title: {updatedEvent.Summary}\n" +
                   $"  ID: {updatedEvent.Id}\n" +
                   $"  Start: {updatedEvent.Start?.DateTimeDateTimeOffset?.ToString("yyyy-MM-dd HH:mm") ?? updatedEvent.Start?.Date}\n" +
                   $"  End: {updatedEvent.End?.DateTimeDateTimeOffset?.ToString("yyyy-MM-dd HH:mm") ?? updatedEvent.End?.Date}";
        }
        catch (Exception ex)
        {
            return $"Error updating calendar event: {ex.Message}";
        }
    }
}
