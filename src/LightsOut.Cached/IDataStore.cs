using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LightsOut.Models;

namespace LightsOut.Cached
{
    /// <summary>
    /// Represents a data store to use with the cached api. The default
    /// will be backed by sqlite - implement your own if you need to.
    /// </summary>
    public interface IDataStore
    {
        /// <summary>
        /// Initialise the database, if not already initialised
        /// </summary>
        /// <returns></returns>
        Task Initialise();

        /// <summary>
        /// Save fetched status
        /// </summary>
        /// <param name="status"></param>
        Task SaveStatus(
            Dictionary<string, StatusItem> status
        );

        /// <summary>
        /// Offline status search
        /// </summary>
        /// <returns></returns>
        Task<Dictionary<string, StatusItem>> FetchStatus();

        /// <summary>
        /// Save areas to the cache
        /// </summary>
        /// <param name="areas"></param>
        Task SaveAreas(
            Area[] areas
        );

        /// <summary>
        /// Offline area search
        /// </summary>
        /// <param name="search"></param>
        Task<Area[]> SearchAreas(
            string search
        );

        /// <summary>
        /// Save fetched areas to the cache
        /// </summary>
        /// <param name="areaId"></param>
        /// <param name="areaSchedule"></param>
        /// <returns></returns>
        Task SaveAreaSchedule(
            string areaId,
            AreaSchedule areaSchedule
        );

        /// <summary>
        /// Offline area schedule fetch
        /// </summary>
        /// <param name="areaId"></param>
        /// <returns></returns>
        Task<AreaSchedule> FetchAreaSchedule(
            string areaId
        );

        /// <summary>
        /// Test if area schedules are out of date
        /// - returns true if there's no cache item for the area
        /// - returns true if the lifetime of cached data for the
        ///     area has exceeded maxAge
        /// </summary>
        /// <param name="areaId"></param>
        /// <param name="maxAge"></param>
        /// <returns></returns>
        Task<bool> IsAreaScheduleMissingOrStale(
            string areaId,
            TimeSpan maxAge
        );
    }
}