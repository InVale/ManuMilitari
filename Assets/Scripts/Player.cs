using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerType
{
    DEBUG,
    LOCAL,
    NETWORK
}

public class Player
{
    public PlayerType MyType;
    public List<Unit> Units { get; set; } = new List<Unit>();

    public bool IsLocal;
}
