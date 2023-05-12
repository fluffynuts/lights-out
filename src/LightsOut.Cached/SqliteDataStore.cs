using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using LightsOut.Cached;
using LightsOut.Models;
using Microsoft.Data.Sqlite;

namespace LightsOut.Cached;

/// <summary>
/// Provides a sqlite-backed datastore
/// </summary>
public class SqliteDataStore : IDataStore
{
    private readonly string _connectionString;

    /// <summary>
    /// Creates the sqlite data store around the provided path
    /// </summary>
    /// <param name="dbPath"></param>
    public SqliteDataStore(
        string dbPath
    )
    {
        var builder = new SqliteConnectionStringBuilder
        {
            DataSource = dbPath
        };
        _connectionString = builder.ToString();
    }

    /// <inheritdoc />
    public async Task Initialise()
    {
        using var conn = await OpenConnection();
        await conn.ExecuteAsync(
            @"
        create table if not exists cache(
            context varchar(128),
            id varchar(128),
            data text,
            lastmodified varchar(32)
        );
"
        );
    }

    /// <inheritdoc />
    public async Task<bool> IsAreaScheduleMissingOrStale(
        string areaId,
        TimeSpan maxAge
    )
    {
        using var conn = await OpenConnection();
        var cacheRow = await FindCacheRow<AreaSchedule>(
            conn,
            CacheContexts.AREA_SCHEDULE,
            areaId
        );
        var all = await FindAllCacheRows<AreaSchedule>(
            conn,
            CacheContexts.AREA_SCHEDULE
        );
        var cutoff = DateTime.Now - maxAge;
        return cacheRow is null ||
            cacheRow.LastModifiedOn < cutoff;
    }

    /// <inheritdoc />
    public async Task SaveStatus(
        Dictionary<string, StatusItem> status
    )
    {
        var cacheRow = CacheRow.For(
            CacheContexts.STATUS,
            status,
            ""
        );
        using var conn = await OpenConnection();
        await SaveCacheRow(
            conn,
            cacheRow
        );
    }

    /// <inheritdoc />
    public async Task<Dictionary<string, StatusItem>> FetchStatus()
    {
        using var conn = await OpenConnection();
        var rows = await FindAllCacheRows<Dictionary<string, StatusItem>>(
            conn,
            CacheContexts.STATUS
        );
        return rows.FirstOrDefault()?.Item;
    }

    /// <inheritdoc />
    public async Task SaveAreas(
        Area[] areas
    )
    {
        using var conn = await OpenConnection();
        foreach (var area in areas)
        {
            await SaveArea(
                conn,
                area
            );
        }
    }

    /// <inheritdoc />
    public async Task<Area[]> SearchAreas(
        string search
    )
    {
        using var conn = await OpenConnection();
        var allAreas = await FindAllCacheRows<Area>(
            conn,
            CacheContexts.AREA
        );
        return allAreas.Where(
                o => o.Id.Contains(search) ||
                    o.Item.Name.Contains(search) ||
                    o.Item.Region.Contains(search)
            ).Select(o => o.Item)
            .ToArray();
    }


    /// <inheritdoc />
    public async Task SaveAreaSchedule(
        string areaId,
        AreaSchedule areaSchedule
    )
    {
        var cacheRow = CacheRow.For(
            CacheContexts.AREA_SCHEDULE,
            areaSchedule,
            areaId
        );
        using var conn = await OpenConnection();
        await SaveCacheRow(
            conn,
            cacheRow
        );
    }

    /// <inheritdoc />
    public async Task<AreaSchedule> FetchAreaSchedule(
        string areaId
    )
    {
        using var conn = await OpenConnection();
        var row = await FindCacheRow<AreaSchedule>(
            conn,
            CacheContexts.AREA_SCHEDULE,
            areaId
        );
        return row?.Item;
    }

    private async Task SaveArea(
        IDbConnection conn,
        Area area
    )
    {
        var cacheRow = CacheRow.For(
            CacheContexts.AREA,
            area,
            area.Id
        );
        await SaveCacheRow(
            conn,
            cacheRow
        );
    }

    private async Task<IEnumerable<CacheRow<T>>> FindAllCacheRows<T>(
        IDbConnection conn,
        string context
    ) where T : class
    {
        return await conn.QueryAsync<CacheRow<T>>(
            "select * from cache where context = @context;",
            new { context = context }
        );
    }

    private async Task<CacheRow<T>> FindCacheRow<T>(
        IDbConnection conn,
        string context,
        string id
    ) where T : class
    {
        return await conn.QueryFirstOrDefaultAsync<CacheRow<T>>(
            "select * from cache where context = @context and id = @id;",
            new { context = context, id = id }
        );
    }

    private static async Task SaveCacheRow(
        IDbConnection conn,
        CacheRow cacheRow
    )
    {
        var shouldUpdate = await CacheRowExists(
            conn,
            cacheRow
        );
        if (shouldUpdate)
        {
            await UpdateCacheRow(
                conn,
                cacheRow
            );
        }
        else
        {
            await InsertCacheRow(
                conn,
                cacheRow
            );
        }
    }

    private static async Task<bool> CacheRowExists(
        IDbConnection conn,
        CacheRow cacheRow
    )
    {
        return await conn.QueryFirstAsync<int>(
            "select count(*) from cache where context = @context and id = @id;",
            new { context = cacheRow.Context, id = cacheRow.Id }
        ) > 0;
    }

    private static async Task UpdateCacheRow(
        IDbConnection conn,
        CacheRow cacheRow
    )
    {
        await conn.ExecuteAsync(
            @"
            update cache set
                data = @data,
                lastmodified = @lastModified
            where
                context = @context
                and
                id = @id;
            ",
            new
            {
                context = cacheRow.Context,
                id = cacheRow.Id,
                data = cacheRow.Data,
                lastModified = cacheRow.LastModified
            }
        );
    }

    private static async Task InsertCacheRow(
        IDbConnection conn,
        CacheRow cacheRow
    )
    {
        await conn.ExecuteAsync(
            @"
            insert into cache (context, id, data, lastmodified)
                        values (@context, @id, @data, @lastModified);
            ",
            new
            {
                context = cacheRow.Context,
                id = cacheRow.Id,
                data = cacheRow.Data,
                lastModified = cacheRow.LastModified
            }
        );
    }

    private async Task<IDbConnection> OpenConnection()
    {
        var result = new SqliteConnection(_connectionString);
        await result.OpenAsync();
        return result;
    }
}