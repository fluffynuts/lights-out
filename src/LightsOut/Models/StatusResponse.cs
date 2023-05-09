using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LightsOut.Models
{
    /// <summary>
    /// The full response for a status request
    /// </summary>
    public class StatusResponse
    {
        /// <summary>
        /// The Status collection should always contain this, at least,
        /// and will likely contain capetown
        /// </summary>
        public const string NATION_WIDE_STATUS = "eskom";
        /// <summary>
        /// The key to find the Cape Town status, where the ANC
        /// has had some of their terrible mismanagement mitigated
        /// by a local ruling party that cares more about constituents
        /// than cadres.
        /// </summary>
        public const string CAPETOWN_STATUS = "capetown";

        /// <summary>
        /// A collection of status items for areas which project
        /// a load-shedding status (typically, "eskom" and "capetown")
        /// </summary>
        [JsonPropertyName("status")]
        public Dictionary<string, StatusItem> Status { get; set; }
    }

    /// <summary>
    /// Each item of the status request
    /// </summary>
    public class StatusItem
    {
        /// <summary>
        /// The name of the status
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The current stage for that status
        /// </summary>
        [JsonPropertyName("stage")]
        public string CurrentStage { get; set; }

        /// <summary>
        /// When this was last updated
        /// </summary>
        [JsonPropertyName("stage_updated")]
        public DateTime StageLastUpdated { get; set; }

        /// <summary>
        /// Projected future stages - in the case of "eskom",
        /// this may be a lot less information than you'll receive
        /// when querying by area - this simply represents the
        /// level of brokenness being currently experienced by
        /// Eishkom.
        /// </summary>
        [JsonPropertyName("next_stages")]
        public StageProjection[] NextStages { get; set; }
    }

    /// <summary>
    /// The projected stage changes for the next 24 hours
    /// </summary>
    public class StageProjection
    {
        /// <summary>
        /// What stage is projected
        /// </summary>
        [JsonPropertyName("stage")]
        public string Stage { get; set; }

        /// <summary>
        /// When it's expected to start
        /// </summary>
        [JsonPropertyName("stage_start_timestamp")]
        public DateTime Starting { get; set; }
    }
}