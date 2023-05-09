Lights Out
---

dotnet Api & tooling around the Eskom Se Push REST api

You will need to obtain an api key from:

https://eskomsepush.gumroad.com/l/api

A personal token with usage limits can be obtained for free.

Usage
---

```csharp
using LightsOut;

public class Program
{
    public async Task Main()
    {
        // create an instance of the api with a token
        // -> if the token is ommitted, the library
        //    will attempt to read it from the environment
        //    variable ESP_API_TOKEN
        var api = new Api("<ESP_API_TOKEN>");
        var overallStatus = await api.FetchStatus();
        Console.WriteLine($"national status: {overallStatus["eskom"].CurrentStage}");

        var areas = await api.SearchAreas("johannesburg")
        var areaId = areas.First().Id;
        var schedule = await api.FetchAreaSchedule(areaId);
        var next = schedule.Events.First();
        Console.WriteLine(
            $"Next demonstration of Eskom ineptitude: {next.Note} at {next.Start} (until {next.End})"
        );
    }
}
```
