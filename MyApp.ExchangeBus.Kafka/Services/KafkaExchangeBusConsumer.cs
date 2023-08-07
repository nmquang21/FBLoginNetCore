using Confluent.Kafka;
using MyApp.Exchange.Models;
using MyApp.ExchangeBus.Kafka.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyApp.Exchange.Bus
{
    public class KafkaExchangeBusConsumer : IExchangeBusConsumer, IDisposable
    {
        private IConsumer<Ignore, string> _consumer;
        private readonly KafkaExchangeBusConfig _config;

        public KafkaExchangeBusConsumer(KafkaExchangeBusConfig config)
        {
            _config = config;
            _consumer = new ConsumerBuilder<Ignore, string>(_config.Consumer).Build();
        }
        public void Dispose()
        {
            if (_consumer != null)
            {
                _consumer.Dispose();
            }
        }
        public async Task<int> Commit()
        {
            _consumer.Commit();
            return 1;
        }

        public async Task<int> StartAsync(CancellationToken cancalToken, Func<ExchangeModel, Task<int>> process)
        {
            var topic = _config.Topic;

            _consumer.Subscribe(topic);

            while (!cancalToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _consumer.Consume(cancalToken);
                    if (consumeResult != null)
                    {
                        var data = JsonConvert.DeserializeObject<ExchangeModel>(consumeResult.Message.Value);
                        if (data.IsCompressed)
                        {
                            data.Data = UnZip(data.Data);
                        }
                        data.Created = DateTime.Now;
                        Console.WriteLine($"ConsumerId {_consumer.Name}");
                        await process.Invoke(data);
                    }
                }
                catch (ConsumeException ex)
                {
                    Console.WriteLine($"Rejoin Topic {topic}");
                    _consumer.Dispose();
                    _consumer = new ConsumerBuilder<Ignore, string>(_config.Consumer).Build();
                    _consumer.Subscribe(topic);
                }
            }
            _consumer.Close();
            return 1;
        }

        public async Task<int> StartAsync(CancellationToken cancalToken, IExchangeBaseCallback func)
        {
            var topic = _config.Topic;

            _consumer.Subscribe(topic);

            while (!cancalToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _consumer.Consume(cancalToken);
                    if (consumeResult != null)
                    {
                        var data = JsonConvert.DeserializeObject<ExchangeModel>(consumeResult.Message.Value);
                        if (data.IsCompressed)
                        {
                            data.Data = UnZip(data.Data);
                        }
                        data.Created = DateTime.Now;
                        Console.WriteLine($"ConsumerId {_consumer.Name}");
                        await func.DoWork(data);
                    }
                }
                catch (ConsumeException ex)
                {
                    Console.WriteLine($"Rejoin Topic {topic}");
                    _consumer.Dispose();
                    _consumer = new ConsumerBuilder<Ignore, string>(_config.Consumer).Build();
                    _consumer.Subscribe(topic);
                }
            }
            _consumer.Close();
            return 1;
        }

        public string UnZip(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);

            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionLevel.Fastest))
                {
                    CopyTo(gs, mso, bytes.Length);
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
