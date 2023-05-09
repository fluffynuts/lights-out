using System;
using System.Linq;
using System.Threading.Tasks;
using LightsOut.Models;
using NUnit.Framework;
using static PeanutButter.RandomGenerators.RandomValueGen;
using NExpect;
using PeanutButter.Utils;
using static NExpect.Expectations;

namespace LightsOut.Tests;

[TestFixture]
public class Tests
{
    [TestFixture]
    public class SearchAreas
    {
        [Test]
        public async Task ShouldReturnAreas()
        {
            // Arrange
            var sut = Create();
            // Act
            var result = await sut.SearchAreas("waterfall");
            // Assert
            Expect(result)
                .Not.To.Be.Empty();
        }
    }

    [TestFixture]
    public class FetchStatus
    {
        [Test]
        public async Task ShouldIncludeEskomStatus()
        {
            // Arrange
            var sut = Create();
            // Act
            var result = await sut.FetchStatus();
            // Assert
            Expect(result)
                .To.Contain.Key(StatusResponse.NATION_WIDE_STATUS);
            Expect(result)
                .To.Contain.Key(StatusResponse.CAPETOWN_STATUS);
        }
    }

    [TestFixture]
    public class FetchEvents
    {
        [Test]
        public async Task ShouldFetchUpcomingLoadsheddingEventsForTheProvidedAreaId()
        {
            // Arrange
            var sut = Create();
            // Act
            var result = await sut.FetchAreaSchedule(
                "eskde-2-randburgnuwestcityofjohannesburggauteng"
            );
            // Assert
            Expect(result.Events)
                .Not.To.Be.Empty();
            Expect(result.Info)
                .Not.To.Be.Null();
            Expect(result.Schedule.Days)
                .To.Contain.Only(7)
                .Items();
        }
    }

    private static Api Create()
    {
        return new Api();
    }
}