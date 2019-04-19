using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public Pawn pawn;
    public Transform testObject; // For testing. Delete me later.



	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{
	    Rotation();
	    Movement();
	}

    void Rotation()
    {
        // Create a Plane object
        Plane thePlane = new Plane(Vector3.up, pawn.tf.position);
        
        // Raycast out the camera (at the mouse position) to the plane -- find the distance to the plane
        float distance;
        Ray theRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        thePlane.Raycast(theRay, out distance);
        
        // Find a point on the ray that is "distance" down the ray
        Vector3 targetPoint = theRay.GetPoint(distance);

        // TEMP: Move a test object to that point
        testObject.position = targetPoint;

        // Rotate to look at that point
        pawn.RotateTowards(targetPoint);
    }

    void Movement()
    {
        // Move
        Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
        moveDirection = Vector3.ClampMagnitude(moveDirection, 1.0f);
        moveDirection = pawn.tf.InverseTransformDirection(moveDirection);

        pawn.Move(moveDirection);
    }


}
