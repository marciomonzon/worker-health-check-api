# 🛠️ ASP.NET API Health Check Worker (.NET)

This is a simple .NET Worker Service that periodically checks the health of a API instance and logs the result. 
It follows the Worker Service pattern introduced in .NET Core 8.0.

---

## 🚀 Features

- ✅ Periodic API Health Checks  
- 🪵 Logs status (Healthy/Unhealthy) to console  
- 🛡️ Easily extendable with more health checks  
- 📦 Uses `Microsoft.Extensions.Diagnostics.HealthChecks`

---

## ⚙️ Configuration of the Worker

### `appsettings.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "ApiSettings": {
    "ApiHealthEndpoint": "https://localhost:7286/healthz"
  },
  "WorkerSettings": {
    "SecondsDelay": "60"
  }
}
```

---

## 💻 Usage

The Worker will check the API called ExampleAPI. The endpoint of the api is already set
on worker appsettings.js in the follow key: ApiSettings:ApiHealthEndpoint.

The worker will run the check every 60 seconds. This can be changed in the follow key of worker appsettings.js: WorkerSettings:SecondsDelay.

Run The API called ExampleAPI and the Worker together.
If you are using Visual Studio, you can configure multiple startup projects.
More information: https://learn.microsoft.com/en-us/visualstudio/ide/how-to-set-multiple-startup-projects?view=vs-2022

---

## 🔁 Worker Example (`ApiHealthCheckerWorker.cs`)

```csharp
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
```

---

## 📦 Program Setup (Program.cs)

```csharp
var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<ApiHealthCheckerWorker>();

var host = builder.Build();
host.Run();
```
---

## Diagram

![image](https://github.com/user-attachments/assets/f7235abc-6610-422b-a56d-9802eb4df7d8)


---

## 📌 Notes

- You can expand this worker to report health to an API, email alerts, or retry strategies.
- This project was made only for studying purpose.

---
