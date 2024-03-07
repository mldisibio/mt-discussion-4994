using discussion_4994.Messages;
using MassTransit;

namespace discussion_4994.Consumers;

public class DoFirstStepConsumer(ILogger<DoFirstStepConsumer> logger) : IConsumer<DoFirstStep>
{
    public async Task Consume(ConsumeContext<DoFirstStep> context)
    {
        logger.LogInformation("{Payload} Starting FirstStep", context.Message.Payload);
        await Task.Delay(500);
        await context.Publish(new InvokeBranch(string.Join("|", context.Message.Payload, "DoFirstStep"), context.Message.CorrelationId));
    }
}

