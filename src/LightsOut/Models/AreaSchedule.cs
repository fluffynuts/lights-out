using System;
using System.Linq;
using System.Text.Json.Serialization;
// ReSharper disable ClassNeverInstantiated.Global

namespace LightsOut.Models
{
    
    /// <summary>
    /// A schedule for an area
    /// </summary>
#pragma warning disable CS0618
    public class AreaSchedule: AreaScheduleResponse
#pragma warning restore CS0618
    {
    }

    /// <summary>
    /// The response from a FetchAreaSchedule request
    /// </summary>
    [Obsolete("This is being renamed to AreaSchedule. This type will be removed in a future release.")]
    public class AreaScheduleResponse
    {
        /// <summary>
        /// The area information
        /// </summary>
        [JsonPropertyName("info")]
        public AreaInfo Info { get; set; }

        /// <summary>
        /// Expected load-shedding events, ie
        /// when the load-shedding level is expected
        /// to change, and to what level
        /// </summary>
        [JsonPropertyName("events")]
        public Event[] Events { get; set; }

        /// <summary>
        /// A full schedule of power-out / load-shedding
        /// time ranges for the upcoming week
        /// </summary>
        [JsonPropertyName("schedule")]
        public Schedule Schedule { get; set; }
    }

    /// <summary>
    /// A collection of load-shedding schedules
    /// </summary>
    public class Schedule
    {
        /// <summary>
        /// The projected load-shedding schedule, per-day
        /// </summary>
        [JsonPropertyName("days")]
        public ScheduleDay[] Days { get; set; }

        /// <summary>
        /// The official source of the information (most likely
        /// the municipality for the requested area)
        /// </summary>
        [JsonPropertyName("source")]
        public string Source { get; set; }
    }

    /// <summary>
    /// Represents all load-shedding blocks for a day
    /// </summary>
    public class ScheduleDay
    {
        /// <summary>
        /// The date of the schedule
        /// </summary>
        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        /// <summary>
        /// The name of the day
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The projected load-shedding times
        /// </summary>
        [JsonPropertyName("stages")]
        public TimeRange[][] Stages { get; set; }
    }

    /// <summary>
    /// Represents a range of time from a start to an end
    /// </summary>
    public class TimeRange
    {
        /// <summary>
        /// When this range starts
        /// </summary>
        public TimeSpan Start { get; set; }
        /// <summary>
        /// When this range ends
        /// </summary>
        public TimeSpan End { get; set; }

        /// <summary>
        /// Displays the range in the format {start}-{end} with
        /// each time represented as {hh:mm}
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Start:hh\\:mm}-{End:hh\\:mm}";
        }

        /// <summary>
        /// Parse an input time range (eg 10:00-12:00)
        /// - will throw if unable to parse
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static TimeRange Parse(string input)
        {
            return TryParse(input, out var result)
                ? result
                : throw new ArgumentException(
                    $"input must be of the format 'hh:mm-hh:mm' (received {input})"
                );
        }

        /// <summary>
        /// Attempt to parse an input time range (eg 10:00-12:00)
        /// </summary>
        /// <param name="input"></param>
        /// <param name="parsed"></param>
        /// <returns></returns>
        public static bool TryParse(string input, out TimeRange parsed)
        {
            var parts = input.Split('-')
                .Select(s => s.Trim())
                .ToArray();
            parsed = default;
            if (parts.Length < 2)
            {
                return false;
            }

            try
            {
                parsed = new TimeRange
                {
                    Start = TimeSpan.Parse(parts[0]),
                    End = TimeSpan.Parse(parts[1])
                };
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Provides some information about an area - returned
    /// as part of an area request, will be missing the
    /// area id as that was required to make the request.
    /// </summary>
    public class AreaInfo
    {
        /// <summary>
        /// The name of the area, including the block,
        /// eg "Waterfall (8)"
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The region the area is found in
        /// </summary>
        [JsonPropertyName("region")]
        public string Region { get; set; }
    }

    /// <summary>
    /// A projected load-shedding event, containing
    /// the start time, end time and a note, typically
    /// something like "Stage 6"
    /// </summary>
    public class Event
    {
        /// <summary>
        /// Start time of the event
        /// </summary>
        [JsonPropertyName("start")]
        public DateTime Start { get; set; }

        /// <summary>
        /// End time of the event
        /// </summary>
        [JsonPropertyName("end")]
        public DateTime End { get; set; }

        /// <summary>
        /// Description of the event, eg "Stage 6"
        /// </summary>
        [JsonPropertyName("note")]
        public string Note { get; set; }
    }
}