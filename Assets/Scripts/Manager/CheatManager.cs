using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatManager : MonoBehaviour
{
    public CheatObject Cheats;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[CreateAssetMenu(fileName = "Cheats", menuName = "Cheats")]
public class CheatObject : ScriptableObject
{
    [NonSerialized]
    public CheatManager manager;

    public bool FogOfWar;

    void OnValidate()
    {

    }
}
