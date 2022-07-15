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
    private static readonly Dictionary<SearchFilter, object> s_entries = new Dictionary<SearchFilter, object>();

    private struct SearchFilter : IEquatable<SearchFilter>
    {
        public readonly Type Type;
        public readonly Component Target;

        public SearchFilter(Type type, Component target = null)
        {
            Type = type;
            Target = target;
        }

        public bool Equals(SearchFilter other)
        {
            return (Type == other.Type && 
                Target == other.Target);
        }
    }

    /// <summary>
    /// Look up a reference using its type as a locator.
    /// </summary>
    /// <typeparam name="T">
    /// A type derived from <see cref="UnityObject"/>.
    /// </typeparam>
    /// <param name="target">
    /// The target that hosts the component.
    /// </param>
    /// <returns>
    /// Returns null if there are no instances of the given type. Otherwise,
    /// returns the last looked up instance of the given type.
    /// </returns>
    public static T Lookup<T>() where T : UnityObject
    {
        Type type = typeof(T);
        var filter = new SearchFilter(type);

        if (!Contains(filter))
        {
            T reference = TryFind<T>(filter);
            TryAdd(reference);
        }
        else if (s_entries[filter] == null)
        {
            s_entries[filter] = TryFind<T>(filter);
        }

        return (T)s_entries[filter];
    }

    /// <summary>
    /// Look up a reference using its type as a locator.
    /// </summary>
    /// <typeparam name="T">
    /// A type derived from <see cref="UnityObject"/>.
    /// </typeparam>
    /// <param name="target">
    /// The target that hosts the component.
    /// </param>
    /// <returns>
    /// Returns null if there are no instances of the given type. Otherwise,
    /// returns the last looked up instance of the given type.
    /// </returns>
    public static T Lookup<T>(Component target) where T : Component
    {
        Type type = typeof(T);
        var filter = new SearchFilter(type, target);

        if (!Contains(filter))
        {
            T reference = TryFind<T>(filter);
            TryAdd(reference, target);
        }
        else if (s_entries[filter] == null)
        {
            s_entries[filter] = TryFind<T>(filter);
        }

        return (T)s_entries[filter];
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
    public static bool TryAdd<T>(T reference, Component target = null) where T : UnityObject
    {
        var key = new SearchFilter(typeof(T), target);
        if (Contains(key))
            return false;

        s_entries.Add(key, reference);

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
    public static bool Contains<T>(Component target = null) where T : Component
    {
        var type = typeof(T);
        var key = new SearchFilter(type, target);
        return Contains(key);
    }

    private static bool Contains(SearchFilter key)
    {
        return s_entries.ContainsKey(key);
    }

    private static T TryFind<T>(SearchFilter filter) where T : UnityObject
    {
        T reference = null;

        if (filter.Target == null)
            reference = UnityObject.FindObjectOfType<T>();
        else
            reference = filter.Target.GetComponentInChildren<T>();

        return reference;
    }
}