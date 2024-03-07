using MassTransit;

namespace discussion_4994.Sagas;

public class PrimarySaga : SagaStateMachineInstance, ISagaVersion
{
    public Guid CorrelationId { get; set; }
    public int Version { get; set; }
    public string CurrentState { get; set; } = default!;

    public string MyPayload { get; set; } = "Empty";
}
