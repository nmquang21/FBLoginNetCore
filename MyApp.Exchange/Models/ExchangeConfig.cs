using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Exchange.Models
{
    public class ExchangeConfig
    {
        public ExchangeinboxConfig Inbox { get; set; }
    }
    /// <summary>
    /// Caaus hinhf 
    /// </summary>
    public class ExchangeinboxConfig
    {
        public string Type { get; set; }
    }
}
