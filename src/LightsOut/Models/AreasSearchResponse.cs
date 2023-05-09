using System.Text.Json.Serialization;

namespace LightsOut.Models
{
    /// <summary>
    /// The full response type for an areas search
    /// </summary>
    public class AreasSearchResponse
    {
        /// <summary>
        /// The areas returned for this query
        /// </summary>
        [JsonPropertyName("areas")]
        public Area[] Areas { get; set; }
    }

    /// <summary>
    /// Individual area items within the areas search response
    /// </summary>
    public class Area
    {
        /// <summary>
        /// The id of the area - this is used to query
        /// area schedule via FetchAreaSchedule
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The name of the area, including the load-shedding
        /// block, eg "Waterfall (8)"
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The region this area is found in
        /// </summary>
        [JsonPropertyName("region")]
        public string Region { get; set; }
    }
}