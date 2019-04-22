using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RagdollControls : MonoBehaviour {

    public GameObject objectToApplyRagdoll;
    // Things to turn off when we ragdoll, on when normal
    public Collider mainCollider;
    public Animator anim;
    public Rigidbody mainRigidbody;
    public NavMeshAgent agent;
    // Things to turn on when ragdoll, off when normal
    public List<Rigidbody> partRigidbodies;
    public List<Collider> partColliders;

	// Use this for initialization
	void Start () {
        mainCollider = objectToApplyRagdoll.GetComponent<Collider>();
        anim = objectToApplyRagdoll.GetComponent<Animator>();
        mainRigidbody = objectToApplyRagdoll.GetComponent<Rigidbody>();
        agent = objectToApplyRagdoll.GetComponent<NavMeshAgent>();

        partRigidbodies = new List<Rigidbody>(objectToApplyRagdoll.GetComponentsInChildren<Rigidbody>());
        partColliders = new List<Collider>(objectToApplyRagdoll.GetComponentsInChildren<Collider>());

        // Deactivate the Ragdoll
        DeactivateRagdoll();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.P))
        {
            ActivateRagdoll();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            DeactivateRagdoll();
        }
	}

    public void ActivateRagdoll ()
    {
        // Turn on all the child rigidbodies
        foreach (Rigidbody rb in partRigidbodies)
        {
            rb.isKinematic = false;
        }
        // Turn on all the child coliders 
        foreach (Collider col in partColliders)
        {
            col.enabled = true;
        }
        // Turn OFF the main stuff
        mainCollider.enabled = false;
        mainRigidbody.isKinematic = true;
        anim.enabled = false;
        if (agent)
        {
            agent.enabled = false;
        }
    }

    public void DeactivateRagdoll()
    {
        // Turn OFF the ragdoll colliders
        foreach (Collider col in partColliders)
        {
            col.enabled = false;
        }
        // Turn OFF the ragdoll rigidbodies
        foreach (Rigidbody rb in partRigidbodies)
        {
            rb.isKinematic = true;
        }
        // Turn ON the main stuff
        mainCollider.enabled = true;
        mainRigidbody.isKinematic = false;
        if (agent)
        {
            agent.enabled = true;
        }
        anim.enabled = true;
    }
}
