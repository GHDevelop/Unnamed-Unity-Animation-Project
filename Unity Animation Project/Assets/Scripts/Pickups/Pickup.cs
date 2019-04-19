using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base Pickup Class. All other pickups should inherit from this.
/// </summary>
public abstract class Pickup : MonoBehaviour
{
    [Header("Edit Friendly")]

    [Header("Movement")]
    [Tooltip("Amount the pickup rotates while in the world."),
        SerializeField]
    private float _rotationAmount = 50.0f;
    public float RotationAmount
    {
        get { return _rotationAmount; }
        protected set { _rotationAmount = value; }
    }

    [Header("Expiration")]
    [Tooltip("Duration the pickup will last for before despawning. \nIrrelevant if expires is false"),
        SerializeField]
    private float _lifetime;
    public float Lifetime
    {
        get { return _lifetime; }
        protected set { _lifetime = value; }
    }

    [Tooltip("Whether the pickup expires after dropping"),
        SerializeField]
    private bool _expires = false;
    public bool Expires
    {
        get { return _expires; }
        protected set { _expires = value; }
    }

    [Header("Viewing Only")]

    [Tooltip("Only visible in the inspector to make sure it isn't null. \nDO NOT EDIT"),
        SerializeField]
    private Transform _myTransform;
    public Transform MyTransform
    {
        get { return _myTransform; }
        protected set { _myTransform = value; }
    }


    /// <summary>
    /// Just gets the pickup's transform when spawned
    /// </summary>
    protected virtual void Awake()
    {
        MyTransform = GetComponent<Transform>();
    }

    /// <summary>
    /// Starts a self destruct timer if the object expires
    /// </summary>
    protected virtual void Start()
    {
        if (Expires)
        {
            Destroy(gameObject, Lifetime);
        }
    }

    /// <summary>
    /// Rotates while the object is in the overworld
    /// </summary>
    protected virtual void Update()
    {
        if (GameManager.BowBeforeMe.Paused)
        {
            return;
        }
        MyTransform.Rotate(MyTransform.up, RotationAmount * Time.deltaTime);
    }

    /// <summary>
    /// Runs whatever the child pickup does when collected.
    /// </summary>
    /// <param name="other"></param>
    protected void OnTriggerEnter(Collider other)
    {
        OnCollect(other);
    }

    protected abstract void OnCollect(Collider other);
}
