using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetManager : MonoBehaviour
{
    public static AssetManager Instance;

    [Header("UI")]
    public GameObject UIAbilityHolder;
    public GameObject UIAbilityButton;

    public void Awake()
    {
        if (!Instance)
            Instance = this;
        else
            Destroy(this);
    }
}
