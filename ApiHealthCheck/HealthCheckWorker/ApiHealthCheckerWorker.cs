namespace HealthCheckWorker
{
    public class ApiHealthCheckerWorker(ILogger<ApiHealthCheckerWorker> _logger,
                                       IConfiguration _configuration) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var httpClient = new HttpClient();
            var apiEndpoint = _configuration["ApiSettings:ApiHealthEndpoint"];
            var secondsDelay = _configuration["WorkerSettings:SecondsDelay"];

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var response = await httpClient.GetAsync(apiEndpoint, stoppingToken);
                    if (response.IsSuccessStatusCode)
                    {
                        _logger.LogInformation("The API is Healthy. ({Time})", DateTimeOffset.Now);
                    }
                    else
                    {
                        _logger.LogWarning("The API is unhealthy. ({StatusCode}) ({Time})",
                                           response.StatusCode,
                                           DateTimeOffset.Now);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error on checking the Healthy of the API ({Time})", DateTimeOffset.Now);
                }

                await Task.Delay(TimeSpan.FromSeconds(int.Parse(secondsDelay!)), stoppingToken);
            }
        }
    }
}
