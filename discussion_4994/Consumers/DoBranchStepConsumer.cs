using discussion_4994.Messages;
using MassTransit;

namespace discussion_4994.Consumers;

public class DoBranchStepConsumer(ILogger<DoBranchStepConsumer> logger) : IConsumer<DoBranchStep>
{
    public async Task Consume(ConsumeContext<DoBranchStep> context)
    {
        logger.LogInformation("{Payload} Starting Branch Step", context.Message.Payload);
        await Task.Delay(500);
        await context.Publish(new InvokeJob(string.Join("|", context.Message.Payload, "DoBranchStep"), context.Message.CorrelationId));
    }
}
