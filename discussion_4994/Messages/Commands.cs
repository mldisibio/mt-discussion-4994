using MassTransit;

namespace discussion_4994.Messages;

public record StartPipeline(string Payload) : CorrelatedBy<Guid>
{
    public Guid CorrelationId { get; set; } = NewId.NextGuid();
}

public record DoFirstStep(string Payload, Guid CorrelationId) : CorrelatedBy<Guid>;

public record InvokeBranch(string Payload, Guid CorrelationId) : CorrelatedBy<Guid>;


public record StartBranch(string Payload, Guid ParentId) : CorrelatedBy<Guid>
{
    public Guid CorrelationId { get; set; } = NewId.NextGuid(); 
}

public record DoBranchStep(string Payload, Guid CorrelationId) : CorrelatedBy<Guid>;

public record InvokeJob(string Payload, Guid CorrelationId) : CorrelatedBy<Guid>;

public record DoJobStep(string Payload, Guid CorrelationId) : CorrelatedBy<Guid>;
