using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyApp.Exchange.Bus;
using MyApp.Exchange.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyApp.ExchangeDataWorker
{
    public class Worker : BackgroundService
    {
        private readonly WorkerMultiThreadConfig _workerMultiThreadConfig;
        private readonly IServiceScope _serviceScope;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger, IServiceScope serviceScope, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceScope = serviceScope;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            for (int i = 0; i < _workerMultiThreadConfig.ThreadCount; i++)
            {
                MakeTask(i, stoppingToken);
                await Task.Delay(_workerMultiThreadConfig.ThreadDelayMiliseconds, stoppingToken);
            }
        }
        public void MakeTask(int i, CancellationToken stoppingToken)
        {
            var exchangeBus = _serviceProvider.GetRequiredService<IExchangeBusConsumer>();
            _ = Task.Run(async () =>
            {
                await exchangeBus.StartAsync(stoppingToken, async (ExchangeModel model) =>
                {
                    try
                    {
                        await exchangeBus.Commit();
                        switch (model.Action)
                        {
                            //case ExchangeAction.BackupDB:

                            //    break;
                        }
                    }
                    catch(Exception ex)
                    {
                        //log
                    }
                    return 1;
                });

            });
        }
    }
}
