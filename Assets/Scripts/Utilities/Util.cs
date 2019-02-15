using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Usefull functions
/// </summary>
public class Util
{
    /// <summary>
    /// Throw an exception and stops the editor if the expression is false
    /// </summary>
    /// <param name="obj"> expression checked </param>
    /// <param name="debugText"> Debug text thrown with the exception</param>
    public static void EditorAssert(bool expression, string debugText)
    {
#if UNITY_EDITOR
        if (!expression)
        {
            EditorApplication.isPlaying = false;
            throw new System.Exception(debugText);
        }
#endif

    }
}

public class Pair<T, U>
{
    public Pair(T first, U second)
    {
        this.First = first;
        this.Second = second;
    }

    public T First { get; set; }
    public U Second { get; set; }
};