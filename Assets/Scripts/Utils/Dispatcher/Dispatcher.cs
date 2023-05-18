using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public class Dispatcher
    {
        private readonly IDictionary<EventId, Action<EventArgs>> _eventHandlers =
            new Dictionary<EventId, Action<EventArgs>>();

        public void Send(EventId eventId, EventArgs args)
        {
            if (!_eventHandlers.TryGetValue(eventId, out var handler))
            {
                Debug.LogError($"Event id: {eventId} is not registered");
                return;
            }
            
            handler?.Invoke(args);
        }

        public void Subscribe(EventId eventId, Action<EventArgs> act)
        {
            if (!_eventHandlers.ContainsKey(eventId))
            {
                _eventHandlers[eventId] = act;
            }
            else
            {
                _eventHandlers[eventId] += act;
            }
        }

        public void Unsubscribe(EventId eventId, Action<EventArgs> action)
        {
            if (_eventHandlers.ContainsKey(eventId))
            {
                _eventHandlers[eventId] -= action;
            }
        }
    }
}