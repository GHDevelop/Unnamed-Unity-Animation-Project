using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterController : MonoBehaviour
{
    [Header("Parameters")]

    [Header("Speed")]

    [Tooltip("Turn speed is measured in degrees per second")]
    [SerializeField] private float turnSpeed = 180;

    [Header("Animator Variable Names")]
    [SerializeField] private string horizontal = "Horizontal";
    [SerializeField] private string vertical = "Vertical";

    [Header("Axis/Button Names")]
    [SerializeField] private string horizontalAxis = "Horizontal";
    [SerializeField] private string verticalAxis = "Vertical";

    [Header("Partner Objects")]
    [SerializeField] private CanvasManager associatedCanvas;

    [Header("Viewing Only")]

    [Header("Components")]
    [SerializeField] private Animator characterAnimator;
    [SerializeField] private Transform myTransform;
    [SerializeField] private HealthWithSelfDestruct health;

    [Header("Generated Objects")]
    [SerializeField] private Transform mouseObject;

    private void Awake()
    {
        characterAnimator = this.GetComponent<Animator>();
        myTransform = this.GetComponent<Transform>();
        health = this.GetComponent<HealthWithSelfDestruct>();

        if (health != null && associatedCanvas != null)
        {
            health.AssociatedCanvas = this.associatedCanvas;
        }

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
        RunSingleButtonAnimations();
    }

    /// <summary>
    /// Rotates the object to face the mouse. Or more specifically an object that's placed behind the mouse.
    /// </summary>
    private void RotateToFaceMouseObject()
    {
        //Gets the rotation to the object
        Quaternion rotationToLookAtMouseObject = Quaternion.LookRotation(mouseObject.position - myTransform.position, Vector3.up);

        //And rotates a certain amount towards that object
        myTransform.rotation = Quaternion.RotateTowards(myTransform.rotation, rotationToLookAtMouseObject, turnSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Sets the position of the mouse object used in RotateToFaceMouseObject
    /// </summary>
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

    /// <summary>
    /// Handles horizontal and vertical movement using the animation controller.
    /// </summary>
    private void MoveHorizontalVertical()
    {
        characterAnimator.SetFloat(horizontal, Input.GetAxis(horizontalAxis));
        characterAnimator.SetFloat(vertical, Input.GetAxis(verticalAxis));

    }

    /// <summary>
    /// Additional actions executed with a single button. These play animations
    /// </summary>
    private void RunSingleButtonAnimations()
    {
        if (Input.GetKey(KeyCode.E))
        {
            characterAnimator.Play("Drop Kick Attack");
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            characterAnimator.Play("Jump Down");
        }
    }

    /// <summary>
    /// Runs when the object is destroyed, usually by the HealthWithSelfDestruct component
    /// </summary>
    private void OnDestroy()
    {
        //Does nothing, for now.

    }
}
