using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Timers;

namespace BackendService
{
    public class SendValuesService : ISendValuesService
    {
        private Random _random;
        private Dictionary<string, HashSet<int>> _subscriptions;

        IReadOnlyDictionary<string, HashSet<int>> ISendValuesService.Subscriptions => _subscriptions.ToImmutableDictionary();

        public event EventHandler<NewValueRegisteredEventArgs> NewValueRegistered;

        public void LiveVariableValues()
        {
            _random = new Random((int)DateTime.Now.Ticks);
            _subscriptions = new Dictionary<string, HashSet<int>>();

            var t = new Timer(1000);
            t.Elapsed += GenerateNewValueMessages;
            t.AutoReset = true;
            t.Start();
        }

        private void GenerateNewValueMessages(object sender, ElapsedEventArgs e)
        {
            var id = _random.Next(1, 6);
            var value = _random.Next(-10, 80);
            var time = DateTime.UtcNow;

            Debug.WriteLine($"\"ID\":{id},\"Value\":\"{value,3}\",\"Timestamp\":\"{time}\"");

            NewValueRegistered?.Invoke(this, new NewValueRegisteredEventArgs(id, value, time));
        }

        public void SubscribeToVariables(string connectionId, IEnumerable<int> variableIds)
        {
            if (_subscriptions == null)
            {
                _subscriptions = new Dictionary<string, HashSet<int>>();
            }

            if (!_subscriptions.Keys.Any(k => k.Equals(connectionId)))
            {
                _subscriptions.Add(connectionId, new HashSet<int>());
            }

            foreach (var variableId in variableIds)
            {
                _subscriptions[connectionId].Add(variableId);
            }
        }

        public void UnsubscribeFromVariables(string connectionId, IEnumerable<int> variableIds)
        {
            if (_subscriptions == null)
            {
                return;
            }

            foreach (var variableId in variableIds)
            {
                _subscriptions[connectionId]?.Remove(variableId);
            }
        }
    }
}
