using MassTransit;

namespace discussion_4994.Sagas;

public class BranchSaga : SagaStateMachineInstance, ISagaVersion
{
    public Guid CorrelationId { get; set; }
    public int Version { get; set; }
    public string CurrentState { get; set; } = default!;

    public Guid ParentId { get; set; }
    public string MyPayload { get; set; } = "Empty";
}
