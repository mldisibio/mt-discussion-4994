using discussion_4994;
using discussion_4994.Consumers;
using discussion_4994.Messages;
using discussion_4994.Sagas;
using discussion_4994.StateMachines;
using MassTransit;
using StackExchange.Redis;

IHost host = Host.CreateDefaultBuilder(args)
                 .ConfigureLogging(loggingBuilder =>
                 {
                     loggingBuilder.ClearProviders()
                                   .AddSimpleConsole(configure => configure.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ");
                 })
                 .ConfigureServices((builder, services) =>
                 {
                     services.AddMassTransit(bus =>
                     {
                         bus.SetKebabCaseEndpointNameFormatter();
                         bus.AddConsumer<DoFirstStepConsumer>();
                         bus.AddConsumer<DoBranchStepConsumer>();
                         bus.AddConsumer<DoJobStepConsumer>((ctx,cfg) =>
                         {
                             cfg.Options<JobOptions<DoJobStep>>(opts =>
                             {
                                 opts.SetJobTimeout(TimeSpan.FromMinutes(120))
                                     .SetConcurrentJobLimit(1)
                                     .SetRetry(r => r.Interval(3, TimeSpan.FromSeconds(30)));
                             });
                         });

                         string primaryRedisConn = builder.Configuration.GetConnectionString("RedisForPrimaryStateMachine")!;
                         string branchRedisConn = builder.Configuration.GetConnectionString("RedisForBranchStateMachine")!;
                         string jobsRedisConn = builder.Configuration.GetConnectionString("RedisForJobStateMachine")!;

                         bus.AddSagaStateMachine<PrimarySagaStateMachine, PrimarySaga>((regCtx, cfg) =>
                         {
                             cfg.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(3)));
                             cfg.UseInMemoryOutbox(regCtx);
                         })
                         .RedisRepository(cfg =>
                         {
                             // use any of the three defined connection strings; they will all work;
                             // but subsequent state machine redis repositories only use the first defined connection string
                             cfg.ConnectionFactory(() => ConnectionMultiplexer.Connect(primaryRedisConn));
                         });

                         bus.AddSagaStateMachine<BranchSagaStateMachine, BranchSaga>((regCtx, cfg) =>
                         {
                             cfg.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(3)));
                             cfg.UseInMemoryOutbox(regCtx);
                         })
                         .RedisRepository(cfg =>
                         {
                             // ignored; first defined connection string is used
                             cfg.ConnectionFactory(() => ConnectionMultiplexer.Connect(branchRedisConn));
                         });

                         // ------------------------------------------------------------------
                         // for job consumers
                         // ------------------------------------------------------------------
                         bus.AddDelayedMessageScheduler();
                         bus.SetJobConsumerOptions(); 
                         bus.AddJobSagaStateMachines(opts =>
                         {
                            opts.FinalizeCompleted = false; // remove job sagas when done or not
                            opts.SlotWaitTime = TimeSpan.FromSeconds(10);
                            opts.SuspectJobRetryCount = 0;
                         })
                         .RedisRepository(cfg =>
                         {
                             // ignored; first defined connection string is used
                            cfg.ConnectionFactory(() => ConnectionMultiplexer.Connect(jobsRedisConn));
                         });

                         bus.UsingRabbitMq((mt, mq) =>
                         {
                             mq.Host("rabbitmq-4994");
                             mq.UseDelayedMessageScheduler();
                             mq.ConfigureEndpoints(mt);
                         });
                     })
                     .AddOptions<MassTransitHostOptions>().Configure(mtOpts =>
                     {
                         mtOpts.WaitUntilStarted = true;
                         mtOpts.StopTimeout = TimeSpan.FromMinutes(60);
                         mtOpts.ConsumerStopTimeout = TimeSpan.FromSeconds(5);
                     })
                     ;

                     services.AddHostedService<Worker>();
                 })
                 .Build();
await host.RunAsync().ConfigureAwait(false);
