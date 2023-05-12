using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LightsOut.Models;
using SQLitePCL;

[assembly: InternalsVisibleTo("LightsOut.Cached.Tests")]

namespace LightsOut.Cached;

/// <summary>
/// LightsOut Api, with client-side caching via sqlite
/// </summary>
// ReSharper disable once UnusedType.Global
public class CachedApi : IApi
{
    /// <inheritdoc />
    public string Token => _actual.Token;

    /// <summary>
    /// The max age for data before it's considered stale
    /// </summary>
    public TimeSpan DataMaxAge { get; }

    private readonly IApi _actual;
    private readonly IDataStore _dataStore;

    /// <summary>
    /// Create a new CachedApi with the default ttl: 1 hour
    /// </summary>
    /// <param name="token"></param>
    /// <param name="dataStore"></param>
    public CachedApi(
        string token,
        IDataStore dataStore
    ) : this(
        token,
        dataStore,
        TimeSpan.FromHours(1)
    )
    {
    }

    /// <summary>
    /// Create a new CachedApi with the provided ttl
    /// </summary>
    /// <param name="token"></param>
    /// <param name="dataStore"></param>
    /// <param name="dataTtl"></param>
    public CachedApi(
        string token,
        IDataStore dataStore,
        TimeSpan dataTtl
    ) : this(
        new Api(token),
        dataStore,
        dataTtl
    )
    {
    }

    internal CachedApi(
        IApi actual,
        IDataStore dataStore,
        TimeSpan dataMaxAge
    )
    {
        _dataStore = dataStore ?? throw new ArgumentNullException(nameof(dataStore));
        _actual = actual ?? throw new ArgumentNullException(nameof(actual));
        
        DataMaxAge = dataMaxAge;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _actual.Dispose();
    }

    /// <inheritdoc />
    public Task<Area[]> SearchAreas(
        string search
    )
    {
        throw new System.NotImplementedException();
    }

    /// <inheritdoc />
    public Task<Dictionary<string, StatusItem>> FetchStatus()
    {
        throw new System.NotImplementedException();
    }

    /// <inheritdoc />
    public Task<AreaSchedule> FetchAreaSchedule(
        string areaId
    )
    {
        throw new System.NotImplementedException();
    }
}