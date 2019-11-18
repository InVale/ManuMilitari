using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

public enum TurnState
{
    WAITING,
    WAITING_NETWORK,
    ANIMATION
}

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    [NonSerialized] public List<Player> Players = new List<Player>();
    [NonSerialized] public List<Player> LocalPlayers = new List<Player>();
    [NonSerialized] public List<Player> DebugPlayers = new List<Player>();
    [NonSerialized] public List<Player> NetPlayers = new List<Player>();
    [NonSerialized] public int currentLocalPlayer;

    [ReadOnly] public TurnState State;
    [ReadOnly] public int TurnCount = 1;

    void Awake()
    {
        if (Instance)
            Destroy(this);
        else
            Instance = this;
    }

    public void EndPlayerTurn()
    {
        currentLocalPlayer++;
        if (currentLocalPlayer >= LocalPlayers.Count)
            EndTurn();
    }

    public void EndTurn()
    {
        foreach (Player player in Players)
            foreach (Unit unit in player.Units)
                foreach (Ability ability in unit.UnitAbilities)
                    ability.EndVisualization();

        State = TurnState.ANIMATION;
        LogManager.Instance.LogTurn(Players);
        AnimationManager.Instance.PlayAnimation();
    }

    public void NewTurn()
    {
        State = TurnState.WAITING;
        TurnCount++;
        currentLocalPlayer = 0;

        foreach (Player player in Players)
            foreach (Unit unit in player.Units)
                unit.UnitOrder.Abilities.Clear();
    }
}
