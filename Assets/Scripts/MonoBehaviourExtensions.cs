using System;
using System.Collections;
using UnityEngine;

public static class MonoBehaviourExtensions
{
    public static void ExecuteAfterDelay(this MonoBehaviour mb, float delay, Action callback)
    {
        mb.StartCoroutine(DelayedCallback(delay, callback));
    }
    
    private static IEnumerator DelayedCallback(float delay, Action callback)
    {
        yield return new WaitForSeconds(delay);
        callback();
    }
}
