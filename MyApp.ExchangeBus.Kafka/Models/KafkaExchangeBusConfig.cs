using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.ExchangeBus.Kafka.Models
{
    public class KafkaExchangeBusConfig
    {
        public string Topic { get; set; }

        public ProducerConfig Producer { get; set; }

        public ConsumerConfig Consumer { get; set; }

        public int? MaxSizeMessage { get; set; }
    }
}
