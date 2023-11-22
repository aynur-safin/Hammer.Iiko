using Hammer.Iiko.ReservePlugin.Dto;
using Resto.Front.Api;
using Resto.Front.Api.Attributes;
using Resto.Front.Api.Data.Brd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;

namespace Hammer.Iiko.ReservePlugin
{
    [PluginLicenseModuleId(21016318)]
    public class ReservePlugin : IFrontPlugin
    {
        private readonly Stack<IDisposable> subscriptions = new Stack<IDisposable>();
        private readonly ExternalReserveService externalReserveService;

        private List<ReserveDto> ReserveSamples()
        {
            var table = PluginContext.Operations.GetTables().First();

            return new List<ReserveDto> {
                new ReserveDto
                {
                    Id = Guid.Parse("d3ea3b3d-8b31-435d-8c7d-900d9870f57e"),
                    EstimatedStartTime = DateTime.Now.AddHours(1),
                    Client = new ClientDto {
                        Id = Guid.Parse("3759de69-703b-4d64-9a1e-22e4c1191f9d"),
                        Name = "Иван",
                        Phones = new List<Dto.PhoneDto> { new Dto.PhoneDto("+79876543210", true) } },
                    Tables = new List<TableDto> { new TableDto { Id = table.Id } }
                },
                new ReserveDto
                {
                    Id = Guid.Parse("61acb9bb-ecd5-433f-acba-30ba7c1b5d5a"),
                    EstimatedStartTime = DateTime.Now.AddHours(2),
                    Client = new ClientDto {
                        Id = Guid.Parse("c449dc57-0545-41a9-b3f4-8cc3a64150bc"),
                        Name = "Артур",
                        Phones = new List<Dto.PhoneDto> { new Dto.PhoneDto("+79876543211", true) } },
                    Tables = new List<TableDto> { new TableDto { Id = table.Id } }
                },
                new ReserveDto
                {
                    Id = Guid.Parse("dd83fe91-0001-4fc8-a97e-a4192e6f7805"),
                    EstimatedStartTime = DateTime.Now.AddHours(3),
                    Client = new ClientDto {
                        Id = Guid.Parse("e6912037-bee1-468e-ad6a-bdecae3032fa"),
                        Name = "Екатерина",
                        Phones = new List<Dto.PhoneDto> { new Dto.PhoneDto("+79876543212", true) } },
                    Tables = new List<TableDto> { new TableDto { Id = table.Id } }
                }
            };
        }

        public ReservePlugin()
        {
            PluginContext.Log.Info("Initializing Hammer.Iiko.ReservePlugin");

            externalReserveService = new ExternalReserveService();
            subscriptions.Push(new ReserveChangeNotifier(externalReserveService));

            PluginContext.Operations.AddNotificationMessage("Плагин запущен!", "Hammer.Iiko.ReservePlugin", (TimeSpan?)null);

            foreach (var reserveDto in ReserveSamples())
            {
                externalReserveService.ReserveIncoming(reserveDto);
            }
        }

        public void Dispose()
        {
            while (subscriptions.Any())
            {
                var subscription = subscriptions.Pop();
                try
                {
                    subscription.Dispose();
                }
                catch (RemotingException ex)
                {
                    PluginContext.Log.Warn($"RemotingException on Dispose: {ex.Message}");
                }
            }

            PluginContext.Log.Info("Hammer.Iiko.ReservePlugin stopped");
        }
    }
}
