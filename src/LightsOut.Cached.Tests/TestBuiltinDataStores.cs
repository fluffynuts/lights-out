using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LightsOut.Models;
using static PeanutButter.RandomGenerators.RandomValueGen;
using NExpect;
using PeanutButter.Utils;
using static NExpect.Expectations;

namespace LightsOut.Cached.Tests;

[TestFixture]
public class TestSqliteDataStore : TestDataStore<SqliteDataStore>
{
    public TestSqliteDataStore()
    {
        TestArenaFactory = async () =>
        {
            var result = new SqliteDataStoreTestArena();
            await result.Initialise();
            return result;
        };
    }

    private class SqliteDataStoreTestArena : DataStoreTestArena<SqliteDataStore>
    {
        public override async Task Initialise()
        {
            var tempFile = Disposables.Add(new AutoTempFile());
            DataStore = new SqliteDataStore(tempFile.Path);
            await DataStore.Initialise();
        }
    }
}

[TestFixture]
public class TestInMemoryDataStore : TestDataStore<InMemoryDataStore>
{
    public TestInMemoryDataStore()
    {
        TestArenaFactory = async () =>
        {
            var result = new InMemoryDataStoreTestArena();
            await result.Initialise();
            return result;
        };
    }

    private class InMemoryDataStoreTestArena : DataStoreTestArena<InMemoryDataStore>
    {
        public override Task Initialise()
        {
            DataStore = new InMemoryDataStore();
            return Task.CompletedTask;
        }
    }
}

public abstract class TestDataStore<T> where T : IDataStore
{
    [Test]
    [Parallelizable]
    public async Task ShouldSaveAnArea()
    {
        // Arrange
        var area = GetRandom<Area>();
        using var arena = await CreateArena();
        var sut = arena.DataStore;
        // Act
        await sut.SaveAreas(new[] { area });
        var result = await sut.SearchAreas(area.Name);
        // Assert
        Expect(result)
            .To.Contain.Only(1)
            .Matched.By(
                o =>
                    o.Name == area.Name &&
                    o.Id == area.Id &&
                    o.Region == area.Region
            );
    }

    [Test]
    [Parallelizable]
    public async Task ShouldOnlyReturnRelevantAreasForSearch()
    {
        // Arrange
        var a1 = GetRandom<Area>()
            .With(o => o.Name = "AAA")
            .With(o => o.Region = "AAAAA")
            .With(o => o.Id = "aaa");
        var a2 = GetRandom<Area>()
            .With(o => o.Name = "BBB")
            .With(o => o.Region = "BBBBB")
            .With(o => o.Id = "bbb");
        using var arena = await CreateArena();
        var sut = arena.DataStore;
        // Act
        await sut.SaveAreas(new[] { a1, a2 });
        var result = await sut.SearchAreas("a");
        // Assert
        Expect(result)
            .To.Contain.Only(1)
            .Matched.By(o => o.Id == "aaa");
    }

    [Test]
    [Parallelizable]
    public async Task ShouldSaveAndRetrieveStatus()
    {
        // Arrange
        var status = new Dictionary<string, StatusItem>()
        {
            ["eskom"] = GetRandom<StatusItem>(),
            ["capetown"] = GetRandom<StatusItem>()
        };
        using var arena = await CreateArena();
        var sut = arena.DataStore;
        // Act
        await sut.SaveStatus(status);
        var result = await sut.FetchStatus();
        // Assert
        Expect(result)
            .To.Deep.Equal(status);
    }

    [Test]
    [Parallelizable]
    public async Task ShouldSaveAndRetrieveAreaStatus()
    {
        // Arrange
        var areaSchedule = GetRandom<AreaSchedule>();
        var areaId = GetRandomString();
        using var arena = await CreateArena();
        var sut = arena.DataStore;
        // Act
        await sut.SaveAreaSchedule(
            areaId,
            areaSchedule
        );
        var result = await sut.FetchAreaSchedule(
            areaId
        );
        // Assert
        Expect(result)
            .To.Deep.Equal(areaSchedule);
    }

    // Note: cannot use sub-fixtures and generic tests - dotnet test
    //  won't pick up the sub-fixtures.
    // generic tests make for consistent testing across sqlite and
    // in-memory, so I'm picking that
    [Test]
    [Parallelizable]
    public async Task IsAreaScheduleMissingOrStale_ShouldReturnTrueWhenMissing()
    {
        // Arrange
        using var arena = await CreateArena();
        var sut = arena.DataStore;
        var areaId = GetRandomString();
        // Act
        var result = await sut.IsAreaScheduleMissingOrStale(
            areaId,
            TimeSpan.FromMinutes(1)
        );
        // Assert
        Expect(result)
            .To.Be.True();
    }

    [Test]
    [Parallelizable]
    public async Task IsAreaScheduleMissingOrStale_ShouldReturnTrueWhenStale()
    {
        // Arrange
        var areaId = GetRandomString();
        var areaSchedule = GetRandom<AreaSchedule>();
        using var arena = await CreateArena();
        var sut = arena.DataStore;
        // Act
        await sut.SaveAreaSchedule(
            areaId,
            areaSchedule
        );
        await Task.Delay(TimeSpan.FromSeconds(1.5));
        var result = await sut.IsAreaScheduleMissingOrStale(
            areaId,
            TimeSpan.FromSeconds(1)
        );
        // Assert
        Expect(result)
            .To.Be.True();
    }

    [Test]
    [Parallelizable]
    public async Task IsAreaScheduleMissingOrStale_ShouldReturnFalseWhenFreshEnough()
    {
        // Arrange
        var areaId = GetRandomString();
        var schedule = GetRandom<AreaSchedule>();
        using var arena = await CreateArena();
        var sut = arena.DataStore;

        // Act
        await sut.SaveAreaSchedule(
            areaId,
            schedule
        );
        await Task.Delay(TimeSpan.FromSeconds(1));
        var result = await sut.IsAreaScheduleMissingOrStale(
            areaId,
            TimeSpan.FromMinutes(1)
        );
        // Assert
        Expect(result)
            .To.Be.False();
    }

    private static Task<DataStoreTestArena<T>> CreateArena()
    {
        if (TestArenaFactory is null)
        {
            throw new InvalidOperationException(
                $"{nameof(TestArenaFactory)} not set"
            );
        }

        return TestArenaFactory();
    }

    protected static Func<Task<DataStoreTestArena<T>>> TestArenaFactory;

    public abstract class DataStoreTestArena<T>
        : IDisposable
        where T : IDataStore
    {
        public T DataStore { get; protected set; }
        protected AutoDisposer Disposables;

        public DataStoreTestArena()
        {
            Disposables = new AutoDisposer();
        }

        public abstract Task Initialise();

        public void Dispose()
        {
            Disposables?.Dispose();
            Disposables = null;
        }
    }
}