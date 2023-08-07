using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Exchange.Models
{
    public class ExchangeModel
    {
        public Guid Id { get; set; }

        public string ContextData { get; set; }
        public string Action { get; set; }
        public string DataType { get; set; }
        public string Type { get; set; }
        public string Data { get; set; }
        public string ExchangeType { get; set; }
        public bool IsCompressed { get; set; }
        public DateTime Created { get; set; }
    }
}
