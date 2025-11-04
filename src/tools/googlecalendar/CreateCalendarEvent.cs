// src/tools/googlecalendar/CreateCalendarEvent.cs
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using System;

namespace AISlop;

public class CreateCalendarEvent : ITool
{
    public string Name => "create_calendar_event";

    public async Task<string> ExecuteAsync(Dictionary<string, string> args, ToolExecutionContext context)
    {
        try
        {
            var service = await GoogleCalendarHelper.GetServiceAsync();

            string? calendarId = args.GetValueOrDefault("calendar_id") ?? "primary";
            string? summary = args.GetValueOrDefault("summary");
            string? description = args.GetValueOrDefault("description");
            string? location = args.GetValueOrDefault("location");
            string? startDateTime = args.GetValueOrDefault("start_datetime");
            string? endDateTime = args.GetValueOrDefault("end_datetime");
            string? startDate = args.GetValueOrDefault("start_date");
            string? endDate = args.GetValueOrDefault("end_date");
            bool isAllDay = args.TryGetValue("all_day", out var allDayStr) && bool.TryParse(allDayStr, out var allDay) && allDay;

            if (string.IsNullOrEmpty(summary))
            {
                return "Error: 'summary' parameter is required.";
            }

            Event eventItem = new Event
            {
                Summary = summary,
                Description = description,
                Location = location
            };

            if (isAllDay || (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate)))
            {
                // All-day event
                if (DateTime.TryParse(startDate ?? startDateTime, out var start))
                {
                    eventItem.Start = new EventDateTime
                    {
                        Date = start.ToString("yyyy-MM-dd")
                    };
                }
                else
                {
                    return "Error: Invalid start_date format. Use YYYY-MM-DD.";
                }

                if (DateTime.TryParse(endDate ?? endDateTime, out var end))
                {
                    eventItem.End = new EventDateTime
                    {
                        Date = end.ToString("yyyy-MM-dd")
                    };
                }
                else
                {
                    return "Error: Invalid end_date format. Use YYYY-MM-DD.";
                }
            }
            else
            {
                // Timed event
                if (string.IsNullOrEmpty(startDateTime))
                {
                    return "Error: 'start_datetime' parameter is required for timed events. Format: YYYY-MM-DDTHH:mm:ss or YYYY-MM-DD HH:mm:ss";
                }

                if (string.IsNullOrEmpty(endDateTime))
                {
                    return "Error: 'end_datetime' parameter is required for timed events. Format: YYYY-MM-DDTHH:mm:ss or YYYY-MM-DD HH:mm:ss";
                }

                if (DateTime.TryParse(startDateTime, out var start))
                {
                    // Converti DateTime locale in DateTimeOffset con timezone locale
                    var startOffset = new DateTimeOffset(start, TimeZoneInfo.Local.GetUtcOffset(start));
                    eventItem.Start = new EventDateTime
                    {
                        DateTimeDateTimeOffset = startOffset,
                        TimeZone = TimeZoneInfo.Local.Id
                    };
                }
                else
                {
                    return "Error: Invalid start_datetime format. Use YYYY-MM-DDTHH:mm:ss or YYYY-MM-DD HH:mm:ss";
                }

                if (DateTime.TryParse(endDateTime, out var end))
                {
                    // Converti DateTime locale in DateTimeOffset con timezone locale
                    var endOffset = new DateTimeOffset(end, TimeZoneInfo.Local.GetUtcOffset(end));
                    eventItem.End = new EventDateTime
                    {
                        DateTimeDateTimeOffset = endOffset,
                        TimeZone = TimeZoneInfo.Local.Id
                    };
                }
                else
                {
                    return "Error: Invalid end_datetime format. Use YYYY-MM-DDTHH:mm:ss or YYYY-MM-DD HH:mm:ss";
                }
            }

            var createdEvent = await service.Events.Insert(eventItem, calendarId).ExecuteAsync();

            // Ottieni informazioni sul calendario per debug
            var calendar = await service.Calendars.Get(calendarId).ExecuteAsync();
            var calendarSummary = calendar.Summary ?? calendarId;

            return $"Event created successfully:\n" +
                   $"  Title: {createdEvent.Summary}\n" +
                   $"  ID: {createdEvent.Id}\n" +
                   $"  Calendar: {calendarSummary} ({calendarId})\n" +
                   $"  Start: {eventItem.Start?.DateTimeDateTimeOffset?.ToString("yyyy-MM-dd HH:mm") ?? eventItem.Start?.Date} (Timezone: {eventItem.Start?.TimeZone ?? "N/A"})\n" +
                   $"  End: {eventItem.End?.DateTimeDateTimeOffset?.ToString("yyyy-MM-dd HH:mm") ?? eventItem.End?.Date} (Timezone: {eventItem.End?.TimeZone ?? "N/A"})\n" +
                   $"  HTML Link: {createdEvent.HtmlLink ?? "N/A"}";
        }
        catch (Exception ex)
        {
            return $"Error creating calendar event: {ex.Message}";
        }
    }
}
