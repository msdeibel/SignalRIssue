using System;
using System.Collections.Generic;

namespace BackendService
{
    public interface ISendValuesService
    {
        /// <summary>
        /// Relation of connectionId to subscribed variables
        /// </summary>
        IReadOnlyDictionary<string, HashSet<int>> Subscriptions { get; }

        /// <summary>
        /// Raised after a NewValueMessages was registered.
        /// </summary>
        event EventHandler<NewValueRegisteredEventArgs> NewValueRegistered;

        /// <summary>
        /// Triggers the creation of NewValueMsg objects
        /// </summary>
        void LiveVariableValues();

        /// <summary>
        /// Receive updates on connectionId when any of the variableIds receives a new value
        /// </summary>
        /// <param name="connectionId"></param>
        /// <param name="variableIds"></param>
        void SubscribeToVariables(string connectionId, IEnumerable<int> variableIds);

        /// <summary>
        /// No longer receive updates on connectionId when any of the variableIds receives a new value
        /// </summary>
        /// <param name="connectionId"></param>
        /// <param name="variableIds"></param>
        void UnsubscribeFromVariables(string connectionId, IEnumerable<int> variableIds);
    }
}