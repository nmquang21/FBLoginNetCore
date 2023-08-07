using MyApp.Exchange.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyApp.Exchange.Bus
{
    public interface IExchangeBusConsumer
    {
        Task<int> StartAsync(CancellationToken cancalToken, Func<ExchangeModel, Task<int>> process);

        Task<int> StartAsync(CancellationToken cancalToken,IExchangeBaseCallback func);

        Task<int> Commit();
    }
}
