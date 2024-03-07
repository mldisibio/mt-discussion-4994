using discussion_4994.Messages;
using MassTransit;

namespace discussion_4994.Consumers;

public class DoJobStepConsumer(ILogger<DoJobStepConsumer> logger) : IJobConsumer<DoJobStep>
{
    public async Task Run(JobContext<DoJobStep> context)
    {
        logger.LogInformation("{Payload} Starting Job Step", context.Job.Payload);
        await Task.Delay(2000);
        await context.Publish(new JobOutcome(string.Join("|", context.Job.Payload, "DoJobStep"), context.Job.CorrelationId));
    }
}
