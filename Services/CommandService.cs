using WebCheckerAPI.EntityFrameworkStuff;
using Microsoft.EntityFrameworkCore;
using WebCheckerAPI.Models;

namespace WebCheckerAPI.BackgroundTasks;

public class CommandService : IHostedService
{
	private readonly IServiceScopeFactory _serviceScopeFactory;
	public CommandService(IServiceScopeFactory serviceScopeFactory)
	{
		_serviceScopeFactory = serviceScopeFactory;
	}

	public Task StartAsync(CancellationToken cancellationToken)
	{
		_ = Task.Run(() => ExecuteAsync(cancellationToken), cancellationToken);
		return Task.CompletedTask;
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		Console.WriteLine(" Stop is Working");
		return Task.CompletedTask;
	}
	public async Task ExecuteAsync(CancellationToken cancellationToken)
	{
		Console.WriteLine("ExecuteAsync is Working");
		while (!cancellationToken.IsCancellationRequested)
		{
			using var scope = _serviceScopeFactory.CreateScope();
			await using var db = scope.ServiceProvider.GetRequiredService<RequestDbContext>();

			var now = DateTime.UtcNow;

			var commandObjects = await db.Endpoints.ToListAsync(cancellationToken);
			foreach (var commandObject in commandObjects)
			{
				if (!CheckRequestDate(commandObject.Time, commandObject.LastRequestDate)) continue;

				Console.WriteLine($"Requesting: {commandObject.Url} at {now}");

				var response = await RequestAsync(commandObject.Url);
				if (response.IsSuccessStatusCode)
				{
					var result = new WebCheckerAPI.Models.EndpointResult();

					result.EndpointResultId = Guid.NewGuid();
					result.EndpointId = commandObject.EndpointId;

					result.CreatedTime = DateTime.UtcNow;
					result.State = response.StatusCode.ToString(); ;
					await db.EndpointResults.AddAsync(result, cancellationToken);
				}

				commandObject.LastRequestDate = now;
				Console.WriteLine(response.StatusCode);
			}
			await db.SaveChangesAsync(cancellationToken);
		}
	}

	private bool CheckRequestDate(EndpointTime endpointTime, DateTime lastDate)
	{
		switch (endpointTime)
		{
			case EndpointTime.EveryMinute:
				return lastDate.AddMinutes(1) < DateTime.UtcNow;
			case EndpointTime.EveryTwoMinutes:
				return lastDate.AddMinutes(2) < DateTime.UtcNow;
			case EndpointTime.EveryFiveMinutes:
				return lastDate.AddMinutes(5) < DateTime.UtcNow;
			case EndpointTime.EveryTenMinutes:
				return lastDate.AddMinutes(10) < DateTime.UtcNow;
			default:
				return false;

		}
	}

	private async Task<HttpResponseMessage> RequestAsync(string? url)
	{
		if (url == null)
		{
			return new HttpResponseMessage();
		}
		// Creating  a named client 
		HttpClient client = new HttpClient();

		// HTTP GET request
		HttpResponseMessage response = await client.GetAsync(url);
		return response;

	}
}













