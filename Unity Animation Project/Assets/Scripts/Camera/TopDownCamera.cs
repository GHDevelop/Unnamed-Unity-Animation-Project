using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownCamera : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Transform objectToFollow;
    [SerializeField] private Vector3 followingDistance;

    [Header("Viewing only")]
    [SerializeField] private Transform myTransform;

    // Awake is called when the object is spawned
    private void Awake()
    {
        myTransform = this.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.BowBeforeMe.Paused)
        {
            return;
        }

        if (objectToFollow != null)
        {
            SetCameraPositionToOffsetFromObject();
        }
        else
        {
            objectToFollow = GameManager.BowBeforeMe.Player.gameObject.GetComponent<Transform>();
        }
    }

    private void SetCameraPositionToOffsetFromObject()
    {
        myTransform.position = objectToFollow.position + followingDistance;
    }
}
