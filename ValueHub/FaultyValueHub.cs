using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;

using BackendService;

namespace ValueHub
{
    public class FaultyValueHub : Hub
    {
        private ISendValuesService _sendValuesService;
        private IHubContext<FaultyValueHub> _hubContext;
        private static bool _eventRegistered;

        public FaultyValueHub(ISendValuesService sendValuesService, IHubContext<FaultyValueHub> hubContext)
        {
            _hubContext = hubContext;

            _sendValuesService = sendValuesService;

            _sendValuesService.NewValueRegistered += OnNewValueRegistered;
            _eventRegistered = true;
        }

        private void OnNewValueRegistered(object sender, NewValueRegisteredEventArgs e)
        {
            _ = Task.Run(() => HandleNewValueTask(e));
        }

        private void HandleNewValueTask(NewValueRegisteredEventArgs e)
        {
            var valueMsg = $"\"ID\":{e.Id},\"Value\":\"{e.Value,3}\",\"Timestamp\":\"{e.RegistrationTime}\"";

            Parallel.ForEach(_sendValuesService.Subscriptions,
                new ParallelOptions() { MaxDegreeOfParallelism = 4 },
                subscription =>
                {
                    if (subscription.Value.Contains(e.Id))
                    {
                        _hubContext.Clients.Client(subscription.Key).SendCoreAsync("NewValue", new[] { valueMsg });
                    }
                });
        }

        public async Task SubscribeToVariables(IEnumerable<int> variableIdsToRegister)
        {
            //Save the connectionId since the Context might be disposed before the task is started
            var connectionId = Context.ConnectionId.ToString();

            Debug.WriteLine($"Register variables for connection {connectionId}.");

            await Task.Run(() =>
                           _sendValuesService.SubscribeToVariables(connectionId, variableIdsToRegister)
                        ); 
        }

        public async Task UnsubscribeFromVariables(IEnumerable<int> variableIdsToUnregister)
        {
            //Save the connectionId since the Context might be disposed before the task is started
            var connectionId = Context.ConnectionId.ToString();

            Debug.WriteLine($"Unregister variables for connection {connectionId}.");

            await Task.Run(() =>
                           _sendValuesService.UnsubscribeFromVariables(Context.ConnectionId, variableIdsToUnregister)
                        );
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
