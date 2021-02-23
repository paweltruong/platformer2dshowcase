using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Custom camera followe script if we dont want to use cinemachine (for orthographic)
/// </summary>
[ExecuteInEditMode]
public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform followObject;
    [SerializeField] float distance = -10;
    [SerializeField] float zoom = 5;
    [SerializeField] float smoothing;
    [SerializeField] Vector2 offset;

    Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        var cameraPos = transform.position;
        var targetPos = followObject.position;
        targetPos.x += offset.x;
        targetPos.y += offset.y;
        targetPos.z = distance;

        cam.orthographicSize = zoom;

        transform.position = Vector3.Lerp(cameraPos, targetPos, smoothing * Time.deltaTime);
    }
}
