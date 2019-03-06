using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pickup : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] protected float rotationAmount = 50.0f;

    [Header("Expiration")]
    [SerializeField, Tooltip("Irrelevant if expires is false")] protected float lifetime;
    [SerializeField] protected bool expires = false;

    [HideInInspector] protected Transform myTransform;

    protected virtual void Awake()
    {
        myTransform = GetComponent<Transform>();
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        if (expires)
        {
            Destroy(gameObject, lifetime);
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        myTransform.Rotate(myTransform.up, rotationAmount * Time.deltaTime);
    }

    protected void OnTriggerEnter(Collider other)
    {
        OnCollect(other);
    }

    protected abstract void OnCollect(Collider other);
}
