using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class Pawn : MonoBehaviour
{
    [HideInInspector] public Animator anim;
    [HideInInspector] public Transform tf;
    public float turnSpeed;
    public float moveSpeed;

	// Use this for initialization
	void Start ()
	{
	    anim = GetComponent<Animator>();
	    tf = GetComponent<Transform>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Move(Vector3 direction)
    {
        anim.SetFloat("Vertical", direction.z * moveSpeed);
        anim.SetFloat("Horizontal", direction.x * moveSpeed);
    }

    public void RotateTowards(Vector3 targetPoint)
    {
        Vector3 vectorToLookDown = targetPoint - tf.position;
        Quaternion lookRotation = Quaternion.LookRotation(vectorToLookDown, tf.up);
        tf.rotation = Quaternion.RotateTowards(tf.rotation, lookRotation, turnSpeed * Time.deltaTime);
    }


    public void Rotate(float degrees)
    {
        //tf.Rotate(new Vector3(0.0f, degrees * turnSpeed, 0.0f));
    }


}
