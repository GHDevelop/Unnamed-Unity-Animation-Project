using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ShooterController : MonoBehaviour
{
    [Header("Shooter Controller")]

    [Header("Parameters")]

    [Header("Speed")]

    [Tooltip("Turn speed is measured in degrees per second")]
    [SerializeField] protected float turnSpeed = 180;

    [Header("Animator Variable Names")]
    [SerializeField] protected string horizontal = "Horizontal";
    [SerializeField] protected string vertical = "Vertical";

    [Header("Axis/Button Names")]
    [SerializeField] private string horizontalAxis = "Horizontal";
    [SerializeField] private string verticalAxis = "Vertical";
    [SerializeField] private string fireButton = "Fire1";

    [Header("Partner Objects")]
    [SerializeField] private CanvasManager _associatedCanvas;
    public CanvasManager AssociatedCanvas
    {
        get
        {
            return _associatedCanvas;
        }

        set
        {
            _associatedCanvas = value;
            if (health != null)
            {
                health.AssociatedCanvas = this.AssociatedCanvas;
            }
            if (weaponManager != null)
            {
                weaponManager.AssociatedCanvas = this.AssociatedCanvas;
            }
        }
    }

    [Header("Viewing Only")]

    [Header("Components")]
    [SerializeField] protected Animator characterAnimator;
    [SerializeField] protected Transform myTransform;
    [SerializeField] protected HealthWithSelfDestruct health;
    [SerializeField] protected WeaponManager weaponManager;
    [SerializeField] protected Rigidbody rigidbody;

    [Header("Generated Objects")]
    [SerializeField] private Transform mouseObject;

    protected virtual void Awake()
    {
        characterAnimator = this.GetComponent<Animator>();
        myTransform = this.GetComponent<Transform>();
        health = this.GetComponent<HealthWithSelfDestruct>();
        weaponManager = this.GetComponent<WeaponManager>();
        rigidbody = this.GetComponent<Rigidbody>();

        MakeMouseCursorObject();
    }

    protected virtual void MakeMouseCursorObject()
    {
        mouseObject = new GameObject("Mouse Pointer Object").GetComponent<Transform>();
    }

    //start is used instead of awake to guarantee the game manager loads in first
    protected virtual void Start()
    {
        health.StartInvincibilityOrDestroy(health.MaxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        myTransform.eulerAngles = new Vector3(
            0,
            myTransform.rotation.eulerAngles.y,
            0);

        if (GameManager.Me.Paused)
        {
            return;
        }

        if (GameManager.Me.endCanvas.enabled)
        {
            return;
        }

        MoveHorizontalVertical();
        AdjustPositionOfMouseObject();
        RotateTowardsObject(mouseObject);
        RunSingleButtonAnimations();

        if (Input.GetButtonDown("Fire1"))
        {
            weaponManager.Fire();
        }
    }

    /// <summary>
    /// Rotates the object to face an object. The mouse object for this class, but could be a target for an AI child.
    /// </summary>
    protected void RotateTowardsObject(Transform objectToLookAt)
    {
        //Gets the rotation to the object
        Quaternion rotationToLookAtObject = Quaternion.LookRotation(objectToLookAt.position - myTransform.position, Vector3.up);

        //And rotates a certain amount towards that object
        myTransform.rotation = Quaternion.RotateTowards(myTransform.rotation, rotationToLookAtObject, turnSpeed * Time.deltaTime);
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
    protected virtual void MoveHorizontalVertical()
    {
        characterAnimator.SetFloat(horizontal, Input.GetAxis(horizontalAxis));
        characterAnimator.SetFloat(vertical, Input.GetAxis(verticalAxis));
    }

    /// <summary>
    /// Additional actions executed with a single button. These play animations
    /// </summary>
    private void RunSingleButtonAnimations()
    {
        //Disabled for now
        /*if (Input.GetKey(KeyCode.E))
        {
            characterAnimator.Play("Drop Kick Attack");
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            characterAnimator.Play("Jump Down");
        }*/
    }

    /// <summary>
    /// Runs when the object is destroyed, usually by the HealthWithSelfDestruct component
    /// </summary>
    protected virtual void OnDestroy()
    {
        GameManager.Me.Player = null;
        if (mouseObject)
        {
            Destroy(mouseObject.gameObject);
        }
    }
}
