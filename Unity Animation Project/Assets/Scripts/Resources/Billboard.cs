using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform myTransform;
    private Transform mainCameraTransform;

    // Start is called before the first frame update
    void Start()
    {
        myTransform = GetComponent<Transform>();
        mainCameraTransform = Camera.main.transform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        myTransform.forward = mainCameraTransform.forward;
    }
}
