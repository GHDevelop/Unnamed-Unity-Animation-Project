using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedBoxVisual : MonoBehaviour
{
    public Vector3 scale;
    public float lerpScale = 0.5f;
    public float rayLength = 0.4f;

    public Color cubeColor = Color.red;
    public Color rayColor = Color.blue;

    private void OnDrawGizmos()
    {
        Transform myTransform = GetComponent<Transform>();

        Gizmos.matrix = Matrix4x4.TRS(myTransform.position, myTransform.rotation, Vector3.one);
        Gizmos.color = Color.Lerp(cubeColor, Color.clear, lerpScale);
        Gizmos.DrawCube(Vector3.up * scale.y / 2f, scale);
        Gizmos.color = rayColor;
        Gizmos.DrawRay(Vector3.zero, Vector3.forward * rayLength);
    }
}
