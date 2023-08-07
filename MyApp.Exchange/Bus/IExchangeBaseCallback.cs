using MyApp.Exchange.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Exchange.Bus
{
    public interface IExchangeBaseCallback
    {
        Task DoWork(ExchangeModel exchangeModel);
    }
}
