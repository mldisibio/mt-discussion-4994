using discussion_4994.Messages;
using MassTransit;

namespace discussion_4994;

public class Worker(ILogger<Worker> logger,
                    IBus msgBus) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        msgBus.Publish(new StartPipeline("StartProcess"));
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Worker stopped");
        return Task.CompletedTask;
    }
}
