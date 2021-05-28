using System;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using EventSourcingTaskApp;
using EventSourcingTaskApp.Core;
using EventSourcingTaskApp.Infrastructure;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Task = System.Threading.Tasks.Task;

namespace Consolex
{
    class Program
    {
        private static IEventStoreConnection _connection;
        private static AggregateRepository _aggregateRepository;

        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var serviceCollection = new ServiceCollection();
            var configurationRoot = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            new Startup(configurationRoot).ConfigureServices(serviceCollection);
            
            _aggregateRepository = serviceCollection.BuildServiceProvider().GetRequiredService<AggregateRepository>();

            var newGuid = Guid.NewGuid();
            await Create(newGuid, "name of");

            await Assign(newGuid, "Anders");

            await Move(newGuid, BoardSections.InProgress);
            await Move(newGuid, BoardSections.Done);

            await Complete(newGuid);
        }

        public static async Task Create(Guid id, string title)
        {
            var aggregate = await _aggregateRepository.LoadAsync<EventSourcingTaskApp.Core.Task>(id);
            aggregate.Create(id, title, "Ahmet KÜÇÜKOĞLU");

            await _aggregateRepository.SaveAsync(aggregate);
        }


        public static async Task Assign(Guid id, string assignedTo)
        {
            var aggregate = await _aggregateRepository.LoadAsync<EventSourcingTaskApp.Core.Task>(id);
            aggregate.Assign(assignedTo, "Ahmet KÜÇÜKOĞLU");

            await _aggregateRepository.SaveAsync(aggregate);
        }

        public static async Task Move(Guid id, BoardSections section)
        {
            var aggregate = await _aggregateRepository.LoadAsync< EventSourcingTaskApp.Core.Task >(id);
            aggregate.Move(section, "Ahmet KÜÇÜKOĞLU");

            await _aggregateRepository.SaveAsync(aggregate);
        }

        public static async Task Complete(Guid id)
        {
            var aggregate = await _aggregateRepository.LoadAsync<EventSourcingTaskApp.Core.Task>(id);
            aggregate.Complete("Ahmet KÜÇÜKOĞLU");

            await _aggregateRepository.SaveAsync(aggregate);
            
        }
	}
}
