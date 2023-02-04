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

        private readonly TimeSpan _sleepPeriod = TimeSpan.FromMinutes(1);  // Make configurable!

        public TimedHostedService(ILogger<TimedHostedService> logger, IPodcastBusinessLogicService twitterBusinessLogicService)
        {
            _logger = logger;
            _podcastBusinessLogicService = twitterBusinessLogicService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Timed Hosted Service running.");

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    _logger.LogInformation($"Timed Hosted Service is working (execution every: {_sleepPeriod.TotalMinutes} min).");

                    await _podcastBusinessLogicService.DoWorkAsync(cancellationToken);

                    await Task.Delay(_sleepPeriod, cancellationToken);
                }
            }
            catch (OperationCanceledException ex)
            {
                var cancelledException = ex;  // TODO: Log exception!
            }
            catch (Exception ex)
            {
                var exception = ex;  // TODO: Log exception!
            }
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }
    }
}
