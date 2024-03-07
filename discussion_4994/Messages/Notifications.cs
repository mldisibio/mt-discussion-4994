using MassTransit;

namespace discussion_4994.Messages;

public record BranchOutcome(string Payload, Guid CorrelationId) : CorrelatedBy<Guid>;

public record JobOutcome(string Payload, Guid CorrelationId) : CorrelatedBy<Guid>;
