using Confluent.Kafka;
using MyApp.Exchange.Bus;
using MyApp.Exchange.Models;
using MyApp.ExchangeBus.Kafka.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.ExchangeBus.Kafka.Services
{
    public class KafkaExchangeBusProcedure : IExchangeBusProcedure, IDisposable
    {
        private readonly IProducer<string, string> _producer;
        private readonly KafkaExchangeBusConfig _config;

        public KafkaExchangeBusProcedure(KafkaExchangeBusConfig config)
        {
            _config = config;
            _producer = new ProducerBuilder<string, string>(_config.Producer).Build();
        }
        public void Dispose()
        {
            if (_producer != null)
            {
                _producer.Dispose();
            }
        }

        public async Task PushAsync(ExchangeModel model)
        {
            if(model.Id == Guid.Empty)
            {
                model.Id = Guid.NewGuid();
            }
            //max size
            if(_config.MaxSizeMessage.HasValue && _config.MaxSizeMessage.Value > 0)
            {
                var dataSizeInKb = model.Data.Length / 1024;
                if(dataSizeInKb >= _config.MaxSizeMessage.Value)
                {
                    var messageLength = model.Data == null ? 0 : model.Data.Length;
                    var zipData = Zip(model.Data);
                    model.Data = zipData;
                    model.IsCompressed = true;
                }
            }
            var topic = _config.Topic;

            await _producer.ProduceAsync(topic, new Message<string, string> { Key = model.Id.ToString(), Value = JsonConvert.SerializeObject(model) });
        }

        public string Zip(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);

            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionLevel.Fastest))
                {
                    CopyTo(msi, gs, bytes.Length);
                }
                return Convert.ToBase64String(mso.ToArray());
            }

        }

        public void CopyTo(Stream src, Stream dest, int? length = 0)
        {
            byte[] bytes = new byte[length ?? 4096];
            int cnt;
            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
            {
                dest.Write(bytes, 0, cnt);
            }
        }
    }
}
