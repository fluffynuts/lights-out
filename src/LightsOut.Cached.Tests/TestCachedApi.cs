using System;
using LightsOut.Models;
using NSubstitute;
using PeanutButter.Utils;
using static PeanutButter.RandomGenerators.RandomValueGen;

namespace LightsOut.Cached.Tests;

[TestFixture]
public class TestCachedApi
{
    [TestFixture]
    public class SearchAreas
    {
        [TestFixture]
        public class FirstRequest
        {
            [Test]
            [Explicit("...")]
            public void ShouldQueryFromApiAndCacheResult()
            {
                // Arrange
                var search = GetRandomString();
                var areas = GetRandomArray<Area>();
                var api = Substitute.For<IApi>()
                    .With(a => a.SearchAreas(search).Returns(areas));
                // Act
                // Assert
            }
        }

        private static CachedApi Create(
            IDataStore dataStore,
            IApi api,
            TimeSpan? dataMaxAge = null
        )
        {
            return new CachedApi(
                api,
                dataStore,
                dataMaxAge ?? TimeSpan.FromMinutes(1)
            );
        }
    }
}