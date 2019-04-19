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
    [SerializeField] public bool hitDisjoint = false;

    private void OnCollisionEnter(Collision collision)
    {
        DisjointCheck(collision.collider);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (GameManager.BowBeforeMe.Paused == false)
        {
            DisjointCheck(collision.collider);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        DisjointCheck(other);
    }

    private void OnTriggerStay(Collider other)
    {
        if (GameManager.BowBeforeMe.Paused == false)
        {
            DisjointCheck(other);
        }
    }

    private void DisjointCheck(Collider collider)
    {
        Debug.Log(collider.name);
        if (collider.gameObject.GetComponent<Disjoint>() != null)
        {
            hitDisjoint = true;
        }
        else
        {
            for (int index = 0; index < exempt.Count; index++)
            {
                if (exempt[index].gameObject == collider.gameObject)
                {
                    return;
                }
            }
            hitDisjoint = false;
        }
    }
}
