using System;
using System.Collections.Generic;

namespace FpsDemo.Game
{
    public static class EventManager
    {
        private static readonly Dictionary<Type, Action<GameEvent>> EventSubscribers = new Dictionary<Type, Action<GameEvent>>();

        private static readonly Dictionary<Delegate, Action<GameEvent>> DelegateWrappers = new Dictionary<Delegate, Action<GameEvent>>();

        public static void AddListener<T>(Action<T> evt) where T : GameEvent
        {
            if (!DelegateWrappers.ContainsKey(evt))
            {
                Action<GameEvent> newAction = (e) => evt((T) e);
                DelegateWrappers[evt] = newAction;

                if (EventSubscribers.TryGetValue(typeof(T), out Action<GameEvent> internalAction))
                    EventSubscribers[typeof(T)] = internalAction += newAction;
                else
                    EventSubscribers[typeof(T)] = newAction;
            }
        }

        public static void RemoveListener<T>(Action<T> evt) where T : GameEvent
        {
            if (DelegateWrappers.TryGetValue(evt, out var action))
            {
                if (EventSubscribers.TryGetValue(typeof(T), out var tempAction))
                {
                    tempAction -= action;
                    if (tempAction == null)
                        EventSubscribers.Remove(typeof(T));
                    else
                        EventSubscribers[typeof(T)] = tempAction;
                }

                DelegateWrappers.Remove(evt);
            }
        }

        public static void Broadcast(GameEvent evt)
        {
            if (EventSubscribers.TryGetValue(evt.GetType(), out var action))
                action.Invoke(evt);
        }

        public static void Clear()
        {
            EventSubscribers.Clear();
            DelegateWrappers.Clear();
        }
    }
}