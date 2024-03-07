using discussion_4994.Messages;
using discussion_4994.Sagas;
using MassTransit;

namespace discussion_4994.StateMachines;
public class BranchSagaStateMachine : MassTransitStateMachine<BranchSaga>
{

    public State BranchStarted { get; } = default!;
    public State WaitingOnJob { get; } = default!;

    public Event<StartBranch> StartBranch { get; } = default!;
    public Event<InvokeJob> InvokeJob { get; } = default!;
    public Event<JobOutcome> JobCompleted { get; } = default!;

    public BranchSagaStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => StartBranch);
        Event(() => InvokeJob);
        Event(() => JobCompleted);

        // Define transitions
        Initially(
            When(StartBranch)
                .Then(ctx => 
                { 
                    ctx.Saga.ParentId = ctx.Message.ParentId;
                    ctx.Saga.MyPayload = ctx.Message.Payload; 
                })
                .Publish(ctx => new DoBranchStep(ctx.Message.Payload, ctx.Saga.CorrelationId))
                .TransitionTo(BranchStarted));

        During(BranchStarted,
            When(InvokeJob)
                .Then(ctx => ctx.Saga.MyPayload = ctx.Message.Payload)
                .Publish(ctx => new DoJobStep(ctx.Message.Payload, ctx.Saga.CorrelationId))
                .TransitionTo(WaitingOnJob));

        During(WaitingOnJob,
            When(JobCompleted)
                .Then(ctx => ctx.Saga.MyPayload = string.Join("|", "BRANCH", ctx.Message.Payload))
                .Publish(ctx => new BranchOutcome(ctx.Message.Payload, ctx.Saga.ParentId))
                .Finalize());
    }
}