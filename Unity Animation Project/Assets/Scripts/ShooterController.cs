using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterController : MonoBehaviour
{
    [Header("Parameters")]

    [Header("Speed")]
    /*[SerializeField] private float forwardSpeed = 4;
    [SerializeField] private float backwardSpeed = 2;
    [SerializeField] private float strafeSpeed = 3;*/

    [SerializeField] private float turnSpeed = 180;

    [Header("Animator Variable Names")]
    [SerializeField] private string horizontal = "Horizontal";
    [SerializeField] private string vertical = "Vertical";

    [Header("Axis/Button Names")]
    [SerializeField] private string horizontalAxis = "Horizontal";
    [SerializeField] private string verticalAxis = "Vertical";

    [Header("Viewing Only")]

    [Header("Components")]
    [SerializeField] private Animator characterAnimator;
    [SerializeField] private Transform myTransform;

    [Header("Generated Objects")]
    [SerializeField] private Transform mouseObject;

    private void Awake()
    {
        characterAnimator = this.GetComponent<Animator>();
        myTransform = this.GetComponent<Transform>();
        MakeMouseCursorObject();
    }

    private void MakeMouseCursorObject()
    {
        mouseObject = new GameObject("Mouse Pointer Object").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveHorizontalVertical();
        AdjustPositionOfMouseObject();
        RotateToFaceMouseObject();

        if (Input.GetKey(KeyCode.E))
        {
            characterAnimator.Play("Drop Kick Attack");
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            characterAnimator.Play("Jump Down");
        }
    }

    private void RotateToFaceMouseObject()
    {
        //Gets the rotation to the object
        Quaternion rotationToLookAtMouseObject = Quaternion.LookRotation(mouseObject.position - myTransform.position, Vector3.up);

        //And rotates a certain amount towards that object
        myTransform.rotation = Quaternion.RotateTowards(myTransform.rotation, rotationToLookAtMouseObject, turnSpeed * Time.deltaTime);
    }

    private void AdjustPositionOfMouseObject()
    {
        //Gets the plane the pointer object is on
        Plane characterPlane = new Plane(Vector3.up, mouseObject.position);

        //Gets a ray from the camera going through the mouse position
        Ray theRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        //Gets the point along the ray that is on the pointer object's plane
        float distanceVariable;
        if (characterPlane.Raycast(theRay, out distanceVariable))
        {
            mouseObject.position = theRay.GetPoint(distanceVariable);
        }
    }

    private void MoveHorizontalVertical()
    {
        characterAnimator.SetFloat(horizontal, Input.GetAxis(horizontalAxis));
        characterAnimator.SetFloat(vertical, Input.GetAxis(verticalAxis));

        
    }
}
