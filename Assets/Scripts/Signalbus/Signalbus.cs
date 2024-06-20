using System;
using System.Collections.Generic;

public class Signalbus
{
    static Dictionary<Type, List<object>> subscribers = new();

    public static void Subscirbe<T>(ISubscriber<T> sub) where T : ISignal
    {
        if (!subscribers.ContainsKey(typeof(T)))
        {
            subscribers.Add(typeof(T), new());
        }
        subscribers[typeof(T)].Add(sub);
    }

    public static void Unsubscribe<T>(ISubscriber<T> sub) where T : ISignal
    {
        if (subscribers.ContainsKey(typeof(T)))
        {
            subscribers[typeof(T)].Remove(sub);
        }
    }

    public static void Fire<T>(T signal) where T : ISignal
    {
        if(subscribers.TryGetValue(typeof(T), out var subs))
        {
            foreach(ISubscriber<T> sub in subs)
            {
                sub.OnSignalReceived(signal);
            }
        }
    }
}
