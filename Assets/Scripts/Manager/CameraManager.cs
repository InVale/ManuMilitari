using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    public Camera MainCamera;

    void Awake()
    {
        if (Instance)
            Destroy(this);
        else
            Instance = this;
    }

    void Start ()
    {
        MainCamera = GetComponent<Camera>();
    }
}
