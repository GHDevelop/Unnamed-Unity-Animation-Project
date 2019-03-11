using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A set of additional properties for a collider/trigger. Referenced in HealthWithSelfDesctruct.cs. 
/// Contains damage dealt, whether the hitbox is active, and a list of exempt objects
/// </summary>
public class CollisionsDealDamage : MonoBehaviour
{
    [SerializeField] public float damage;
    [SerializeField] public bool activeHitbox = true;
    [HideInInspector] public List<HealthWithSelfDestruct> exempt;
}
