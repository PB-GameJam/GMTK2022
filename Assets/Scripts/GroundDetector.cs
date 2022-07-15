// Author:  Joseph Crump
// Date:    07/10/22

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component used to track objects that the player is standing on.
/// </summary>
[RequireComponent(typeof(Collider))]
public class GroundDetector : MonoBehaviour
{
    [SerializeField]
    [HideInInspector]
    private Collider _collider;

    [SerializeField]
    private LayerMask _ignoredLayers;

    private HashSet<Collider> _overlapColliders = new HashSet<Collider>();

    /// <summary>
    /// Whether the ground detector is overlapping with any other colliders.
    /// </summary>
    public bool IsGrounded => _overlapColliders.Count > 0;

    private void OnValidate()
    {
        if (_collider == null)
            _collider = GetComponent<Collider>();

        _collider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_ignoredLayers.ContainsLayer(other.gameObject.layer))
            return;

        _overlapColliders.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (_ignoredLayers.ContainsLayer(other.gameObject.layer))
            return;

        _overlapColliders.Remove(other);
    }
}
