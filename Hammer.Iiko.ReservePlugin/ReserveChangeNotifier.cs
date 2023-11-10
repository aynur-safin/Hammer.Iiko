using Resto.Front.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using Resto.Front.Api.Data.Brd;
using Resto.Front.Api.Editors.Stubs;
using Hammer.Iiko.ReservePlugin.Dto;

namespace Hammer.Iiko.ReservePlugin
{
    internal class ReserveChangeNotifier : IDisposable
    {
        private readonly Stack<IDisposable> subscriptions = new Stack<IDisposable>();

        public ReserveChangeNotifier(ExternalReserveService externalReserve)
        {            
            subscriptions.Push(externalReserve.Subscribe(OnExternalReserveIncoming));

            subscriptions.Push(
                PluginContext.Notifications.ReserveChanged
                    .Select(e => e.Entity)
                    .Where(r => r.Status == ReserveStatus.New)
                    .Subscribe(OnReserveCreated));

            subscriptions.Push(
                PluginContext.Notifications.ReserveChanged
                    .Select(e => e.Entity)
                    .Where(r => r.Status == ReserveStatus.Closed)
                    .Subscribe(OnReserveClosed));
        }

        private void OnExternalReserveIncoming(ReserveDto reserveDto)
        {
            if(PluginContext.Operations.TryGetReserveByExternalId(reserveDto.Id) != null)
                return;

            var editSession = PluginContext.Operations.CreateEditSession();

            IClientStub client = PluginContext.Operations.TryGetClientById(reserveDto.Client.Id);
            if (client == null)
            {
                client = editSession.CreateClient(
                    id: reserveDto.Client.Id,
                    name: reserveDto.Client.Name,
                    phones: reserveDto.Client.Phones
                        .Select(p => new Resto.Front.Api.Data.Brd.PhoneDto { PhoneValue = p.PhoneValue, IsMain = p.IsMain })
                        .ToList(),
                    cardNumber: reserveDto.Client.CardNumber,
                    DateTime.Now);
            }

            var tables = PluginContext.Operations.GetTables()
                .Where(t => reserveDto.Tables.Exists(tt => tt.Id == t.Id))
                .ToList();            

            editSession.CreateReserve(reserveDto.EstimatedStartTime, client, tables, externalId: reserveDto.Id);

            PluginContext.Operations.SubmitChanges(editSession);
        }

        private void OnReserveCreated(IReserve reserve)
        {
            PluginContext.Log.Info($"Reserve {reserve.Id} ({reserve.ExternalId}) is created.");
        }

        private void OnReserveClosed(IReserve reserve)
        {
            PluginContext.Log.Info($"Reserve {reserve.Id} ({reserve.ExternalId}) is closed.");
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
        }
    }
}
