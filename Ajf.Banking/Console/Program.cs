using System;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;

namespace Consolex
{
    public class TestEvent
    {
        public string EntityId { get; set; }
        public string ImportantData { get; set; }
    }
    class Program
    {
        private static IEventStoreConnection _connection;

        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

			CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = tokenSource.Token;

            _connection = EventStoreConnection.Create(
                new IPEndPoint(
                    IPAddress.Parse("127.0.0.1"),
                    1113
                )
            );
            await _connection.ConnectAsync();
            
            
            var evt = new TestEvent
            {
                EntityId = Guid.NewGuid().ToString("N"),
                ImportantData = "I wrote my first event!"
            };



            await _connection.AppendToStreamAsync(
                "stream-name",
                ExpectedVersion.Any,
                new EventData(
                    Guid.NewGuid(),
                    evt.GetType().FullName,
                    false,
                    Encoding.UTF8.GetBytes(evt.ToString()),
                    new byte[] { }
                )
            );


            var result = await _connection.ReadStreamEventsForwardAsync(
                "some-stream",
                StreamPosition.Start,1 ,true);

            var events = result.Events;
        }
	}
}
