using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Order 
{
    public int UnitID;
    public List<Tuple<int, Vector2>> Abilities;

    public Order(int unitID)
    {
        UnitID = unitID;
        Abilities = new List<Tuple<int, Vector2>>();
    }
}
