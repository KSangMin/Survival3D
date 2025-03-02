using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util
{
    public static T GetOrAddComponent<T>(this GameObject go) where T : MonoBehaviour
    {
        var component = go.GetComponent<T>();
        if(component != null ) go.AddComponent<T>();
        return component;
    }
}
