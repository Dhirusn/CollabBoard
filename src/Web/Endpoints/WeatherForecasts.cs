using CollabBoard.Application.Common.Models;
using CollabBoard.Application.TodoItems.Queries.GetTodoItemsWithPagination;
using CollabBoard.Application.WeatherForecasts.Queries.GetWeatherForecasts;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CollabBoard.Web.Endpoints;
public class WeatherForecasts : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapGet(GetWeatherForecasts);
    }

    public async Task<Ok<IEnumerable<WeatherForecast>>> GetWeatherForecasts(ISender sender)
    {
        var forecasts = await sender.Send(new GetWeatherForecastsQuery());

        return TypedResults.Ok(forecasts);
    }
}
