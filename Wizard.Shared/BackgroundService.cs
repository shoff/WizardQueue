using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NCrontab;

namespace Wizard.Shared
{
    public abstract class ScheduledProcessor : ScopedProcessor
    {
        private readonly CrontabSchedule schedule;
        private DateTime nextRun;

        protected abstract string Schedule { get; }

        protected ScheduledProcessor(
            ILogger logger,
            IServiceScopeFactory serviceScopeFactory)
            : base(logger, serviceScopeFactory)
        {
            schedule = CrontabSchedule.Parse(Schedule);
            nextRun = schedule.GetNextOccurrence(DateTime.Now);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            do
            {
                var now = DateTime.Now;
                var nextOccurrence = schedule.GetNextOccurrence(now);
                if (now > nextRun)
                {
                    await Process();
                    nextRun = schedule.GetNextOccurrence(DateTime.Now);
                }
                await Task.Delay(5000, stoppingToken); //5 seconds delay
            }
            while (!stoppingToken.IsCancellationRequested);
        }
    }

    public abstract class ScopedProcessor : BackgroundService
    {
        private readonly IServiceScopeFactory serviceScopeFactory;

        protected ScopedProcessor(
            ILogger logger,
            IServiceScopeFactory serviceScopeFactory)
            : base(logger)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }

        protected override Task Process()
        {
            this.logger.LogInformation("ScopedProcessor.Process.");

            using (var scope = serviceScopeFactory.CreateScope())
            {
                return ProcessInScope(scope.ServiceProvider);
            }
        }

        public abstract Task ProcessInScope(IServiceProvider serviceProvider);
    }

    public abstract class BackgroundService : IHostedService
    {
        protected readonly ILogger logger;

        protected BackgroundService(ILogger logger)
        {
            this.logger = logger;
        }

        private Task executingTask;

        private readonly CancellationTokenSource stoppingCts = new CancellationTokenSource();

        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            this.logger.LogInformation("BackgroundService.StartAsync.");

            // Store the task we're executing
            executingTask = ExecuteAsync(stoppingCts.Token);

            // If the task is completed then return it,
            // this will bubble cancellation and failure to the caller
            if (executingTask.IsCompleted)
            {
                return executingTask;
            }

            // Otherwise it's running
            return Task.CompletedTask;
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            this.logger.LogInformation("BackgroundService.StopAsync.");

            // Stop called without start
            if (executingTask == null)
            {
                return;
            }

            try
            {
                // Signal cancellation to the executing method
                stoppingCts.Cancel();
            }
            finally
            {
                // Wait until the task completes or the stop token triggers
                await Task.WhenAny(executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
            }
        }

        protected virtual Task ExecuteAsync(CancellationToken stoppingToken)
        {
            do
            {
                Process();
                Task.Delay(5000, stoppingToken); // 5 seconds delay
            }
            while (!stoppingToken.IsCancellationRequested);

            return Task.CompletedTask;
        }


        public static Task Delay(long milliseconds)
        {
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
            Timer timer = new Timer(_ => tcs.SetResult(null), null, milliseconds, Timeout.Infinite);
            tcs.Task.ContinueWith(c => timer.Dispose());
            return tcs.Task;
        }

        protected abstract Task Process();
    }
}