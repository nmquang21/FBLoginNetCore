using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Exchange.Models
{
    public class WorkerMultiThreadConfig
    {
        /// <summary>
        /// So luowng thread chay
        /// </summary>
        public int ThreadCount { get; set; }
        public int NoDataSleepSeconds { get; set; }
        /// <summary>
        /// delay khi tao thread
        /// </summary>
        public int ThreadDelayMiliseconds { get; set; }

    }
   
}
