// Author:  Joseph Crump
// Date:    07/10/22

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Extension methods for <see cref="LayerMask"/>/
/// </summary>
public static class LayerMaskExtensions
{
    /// <summary>
    /// Check to see whether a layer mask contains the given layer.
    /// </summary>
    public static bool ContainsLayer(this LayerMask layerMask, int layer)
    {
        return (layerMask == (layerMask | (1 << layer)));
    }
}
