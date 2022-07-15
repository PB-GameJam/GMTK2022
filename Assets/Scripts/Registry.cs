// Author:  Joseph Crump
// Date:    07/07/22

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityObject = UnityEngine.Object;

/// <summary>
/// Static class used to find and cache global references to objects.
/// </summary>
public static class Registry
{
    private static readonly Dictionary<Type, object> s_entries = new Dictionary<Type, object>();

    /// <summary>
    /// Look up a reference using its type as a locator.
    /// </summary>
    /// <typeparam name="T">
    /// A type derived from <see cref="UnityObject"/>.
    /// </typeparam>
    /// <returns>
    /// Returns null if there are no instances of the given type. Otherwise,
    /// returns the last looked up instance of the given type.
    /// </returns>
    public static T Lookup<T>() where T : UnityObject
    {
        Type type = typeof(T);

        if (!Contains<T>())
        {
            T reference = TryFind<T>();
            TryAdd(reference);
        }
        else if (s_entries[type] == null)
        {
            s_entries[type] = TryFind<T>();
        }

        return (T)s_entries[type];
    }

    /// <summary>
    /// Add an entry to the registry.
    /// </summary>
    /// <typeparam name="T">
    /// The type used to key the entry for future lookup.
    /// </typeparam>
    /// <param name="reference">
    /// The reference value that is returns when this entry is looked up.
    /// </param>
    /// <returns>
    /// Returns false if the registry already contains this type.
    /// </returns>
    public static bool TryAdd<T>(T reference) where T : UnityObject
    {
        if (s_entries.ContainsKey(typeof(T)))
            return false;

        s_entries.Add(typeof(T), reference);

        return true;
    }

    /// <summary>
    /// Check to see whether the registry contains a certain type.
    /// </summary>
    /// <typeparam name="T">
    /// The type to check for.
    /// </typeparam>
    /// <returns>
    /// Returns true if the registry already contains the given type.
    /// </returns>
    public static bool Contains<T>() where T : UnityObject
    {
        return Contains(typeof(T));
    }

    /// <summary>
    /// Check to see whether the registry contains a certain type.
    /// </summary>
    /// <param name="type">
    /// The type to check for.
    /// </param>
    /// <returns>
    /// Returns true if the registry already contains the given type.
    /// </returns>
    public static bool Contains(Type type)
    {
        return s_entries.ContainsKey(type);
    }

    private static T TryFind<T>() where T : UnityObject
    {
        var reference = UnityObject.FindObjectOfType<T>();

        return reference;
    }
}