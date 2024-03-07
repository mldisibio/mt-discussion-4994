using discussion_4994.Messages;
using discussion_4994.Sagas;
using MassTransit;

namespace discussion_4994.StateMachines;
public class PrimarySagaStateMachine : MassTransitStateMachine<PrimarySaga>
{

    public State WorkRequestSent { get; } = default!;
    public State WaitingOnBranch { get; } = default!;

    public Event<StartPipeline> StartPipeline { get; } = default!;
    public Event<InvokeBranch> InvokeBranch { get; } = default!;
    public Event<BranchOutcome> BranchCompleted { get; } = default!;

    public PrimarySagaStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => StartPipeline);
        Event(() => InvokeBranch);
        Event(() => BranchCompleted);

        // Define transitions
        Initially(
            When(StartPipeline)
                .Then(ctx => ctx.Saga.MyPayload = ctx.Message.Payload)
                .Publish(ctx => new DoFirstStep(ctx.Message.Payload, ctx.Saga.CorrelationId))
                .TransitionTo(WorkRequestSent));

        During(WorkRequestSent,
            When(InvokeBranch)
                .Then(ctx => ctx.Saga.MyPayload = ctx.Message.Payload)
                .Publish(ctx => new StartBranch(ctx.Message.Payload, ctx.Saga.CorrelationId))
                .TransitionTo(WaitingOnBranch));

        During(WaitingOnBranch,
            When(BranchCompleted)
                .Then(ctx => ctx.Saga.MyPayload = string.Join("|", "PRIMARY", ctx.Message.Payload, "BranchCompleted"))
                .Finalize());
    }
}