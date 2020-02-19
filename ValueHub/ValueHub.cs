using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;

using BackendService;

namespace ValueHub
{
    public class ValueHub : Hub
    {
        private ISendValuesService _sendValuesService;
        private IHubContext<ValueHub> _hubContext;
        private static bool _eventRegistered;
        private object _eventRegistrationLock = new object();

        public ValueHub(ISendValuesService sendValuesService, IHubContext<ValueHub> hubContext)
        {
            _hubContext = hubContext;

            _sendValuesService = sendValuesService;

            lock (_eventRegistrationLock)
            {
                if (!_eventRegistered)
                {
                    _sendValuesService.NewValueRegistered += OnNewValueRegistered;
                    _eventRegistered = true;
                }
            }
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
