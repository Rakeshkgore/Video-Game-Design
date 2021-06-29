using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraAspectRatio : MonoBehaviour
{
    public float aspectRatio = 16f / 9f;
    private new Camera camera;

    void Awake()
    {
        camera = GetComponent<Camera>();
    }

    void Start()
    {
        camera.aspect = aspectRatio;
    }
}
