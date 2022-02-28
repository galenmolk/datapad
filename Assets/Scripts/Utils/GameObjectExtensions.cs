using UnityEngine;

public static class GameObjectExtensions
{
    public static T GetOrAddComponent<T>(this GameObject go) where T : Behaviour
    {
        T component = go.GetComponent<T>();
        if (component == null)
            component = go.AddComponent<T>();

        return component;
    }
}
