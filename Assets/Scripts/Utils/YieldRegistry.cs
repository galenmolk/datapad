using System;
using System.Collections.Generic;
using UnityEngine;

public static class YieldRegistry
{
    public static WaitForEndOfFrame WaitForEndOfFrame { get; } = new WaitForEndOfFrame();
    public static WaitForFixedUpdate WaitForFixedUpdate { get; } = new WaitForFixedUpdate();

    private static readonly Dictionary<float, WaitForSeconds> timeIntervalRegistry = new Dictionary<float, WaitForSeconds>();
    private static readonly Dictionary<Func<bool>, WaitUntil> predicateRegistry = new Dictionary<Func<bool>, WaitUntil>();

    public static WaitForSeconds WaitForSeconds(float seconds)
    {
        timeIntervalRegistry.TryGetValue(seconds, out WaitForSeconds yield); 
        return yield ?? RegisterNewWaitForSeconds(seconds);
    }

    public static WaitUntil WaitUntil(Func<bool> predicate)
    {
        predicateRegistry.TryGetValue(predicate, out WaitUntil yield);
        return yield ?? RegisterNewWaitUntil(predicate);
    }

    private static WaitUntil RegisterNewWaitUntil(Func<bool> predicate)
    {
        WaitUntil yield = new WaitUntil(predicate);
        predicateRegistry.Add(predicate, yield);
        return yield;
    }

    private static WaitForSeconds RegisterNewWaitForSeconds(float seconds)
    {
        WaitForSeconds yield = new WaitForSeconds(seconds);
        timeIntervalRegistry.Add(seconds, yield);
        return yield;
    }
}
