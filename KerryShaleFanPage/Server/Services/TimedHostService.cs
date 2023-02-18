using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using KerryShaleFanPage.Server.Interfaces.BusinessLogic;

namespace KerryShaleFanPage.Server.Services
{
    public class TimedHostedService : IHostedService, IDisposable
    {
        private readonly ILogger<TimedHostedService> _logger;  // TODO: Implement logging!
        private readonly IPodcastBusinessLogicService _podcastBusinessLogicService;
        private Timer? _timer = null;

        private readonly TimeSpan _sleepPeriod = TimeSpan.FromMinutes(15);  // Make configurable!

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="twitterBusinessLogicService"></param>
        public TimedHostedService(ILogger<TimedHostedService> logger, IPodcastBusinessLogicService twitterBusinessLogicService)
        {
            _logger = logger;
            _podcastBusinessLogicService = twitterBusinessLogicService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Timed Hosted Service running.");

            _timer = new Timer(DoWork, cancellationToken, TimeSpan.Zero, _sleepPeriod);

            return Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            _timer?.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        private async void DoWork(object? state)
        {
            _logger.LogInformation($"Timed Hosted Service is working (execution every: {_sleepPeriod.TotalMinutes} min).");

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;
            if (state != null && state is CancellationToken)
            {
                cancellationToken = (CancellationToken)state;
            }

            // Ensure we cancel ourselves if the parent is cancelled.
            cancellationToken.ThrowIfCancellationRequested();

            using var childCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            // Set a timeout because sometimes stuff gets stuck.
            childCancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));

            try
            {
                await _podcastBusinessLogicService.DoWorkAsync(childCancellationTokenSource.Token);

            }
            catch (OperationCanceledException ex) when (childCancellationTokenSource.IsCancellationRequested)
            {
                var cancelledException = ex;  // TODO: Log exception!
            }
            catch (OperationCanceledException ex)
            {
                var cancelledException = ex;  // TODO: Log exception!
            }
            catch (Exception ex)
            {
                var exception = ex;  // TODO: Log exception!
            }

            await Task.Delay(_sleepPeriod, cancellationToken);
        }
    }
}
