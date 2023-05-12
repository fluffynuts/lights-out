using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using LightsOut.Models;

namespace LightsOut.Cached;

/// <summary>
/// Provides an in-memory implementation of IDataStore
/// - good for testing or if you only want caching to
///   last for the lifetime of the app
/// </summary>
public class InMemoryDataStore : IDataStore
{
    private readonly ConcurrentDictionary<(string context, string id), (string json, DateTime lastModified)> _store =
        new();

    /// <inheritdoc />
    public Task Initialise()
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task SaveStatus(
        Dictionary<string, StatusItem> status
    )
    {
        _store[(CacheContexts.STATUS, "")] = (
            JsonSerializer.Serialize(status),
            DateTime.Now
        );
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<Dictionary<string, StatusItem>> FetchStatus()
    {
        return Task.FromResult(
            _store.TryGetValue(
                (CacheContexts.STATUS, ""),
                out var result
            )
                ? JsonSerializer.Deserialize<Dictionary<string, StatusItem>>(result.json)
                : null
        );
    }

    /// <inheritdoc />
    public Task SaveAreas(
        Area[] areas
    )
    {
        foreach (var area in areas)
        {
            _store[(CacheContexts.AREA, area.Id)]
                = (JsonSerializer.Serialize(area), DateTime.Now);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<Area[]> SearchAreas(
        string search
    )
    {
        var result = new List<Area>();
        foreach (var kvp in _store)
        {
            if (kvp.Key.context != CacheContexts.AREA)
            {
                continue;
            }

            var area = JsonSerializer.Deserialize<Area>(kvp.Value.json);
            if (
                area.Id.Like(search) ||
                area.Name.Like(search) ||
                area.Region.Like(search)
            )
            {
                result.Add(JsonSerializer.Deserialize<Area>(kvp.Value.json));
            }
        }

        return Task.FromResult(result.ToArray());
    }

    /// <inheritdoc />
    public Task SaveAreaSchedule(
        string areaId,
        AreaSchedule areaSchedule
    )
    {
        _store[(CacheContexts.AREA_SCHEDULE, areaId)] =
            (JsonSerializer.Serialize(areaSchedule), DateTime.Now);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<AreaSchedule> FetchAreaSchedule(
        string areaId
    )
    {
        return Task.FromResult(
            _store.TryGetValue(
                (CacheContexts.AREA_SCHEDULE, areaId),
                out var result
            )
                ? JsonSerializer.Deserialize<AreaSchedule>(result.json)
                : null
        );
    }

    /// <inheritdoc />
    public Task<bool> IsAreaScheduleMissingOrStale(
        string areaId,
        TimeSpan maxAge
    )
    {
        var result = true;
        var cutoff = DateTime.Now - maxAge;
        if (_store.TryGetValue(
                (CacheContexts.AREA_SCHEDULE, areaId),
                out var cached
            ))
        {
            result = cached.lastModified < cutoff;
        }

        return Task.FromResult(result);
    }
}