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
            EndTurnBuffer();
    }

    public void EndTurnBuffer()
    {
        string command = LogManager.Instance.LogTurn(Players);

        if (!TempManager.Instance.Build)
        {
            EndTurn();
            return;
        }

        TempManager.Instance.TurnButton.gameObject.SetActive(false);
        TempManager.Instance.TurnInput.gameObject.SetActive(true);
        TempManager.Instance.TurnOutput.gameObject.SetActive(true);
        TempManager.Instance.TurnOutput.text = command;

        var textEditor = new TextEditor();
        textEditor.text = command;
        textEditor.SelectAll();
        textEditor.Copy();

        State = TurnState.WAITING_NETWORK;
    }

    public void ClearBuffer()
    {
        TempManager.Instance.TurnInput.text = "";
    }

    public void EnterCommand ()
    {
        string[] players = TempManager.Instance.TurnInput.text.Split(";".ToCharArray());
        if (players.Length > 0)
        {
            foreach (Unit unit in Players[1].Units)
                unit.UnitOrder.Abilities.Clear();

            string[] units = players[0].Split("|".ToCharArray());
            for (int i = 0; i < units.Length; i++)
            {
                string[] abilities = units[i].Split("_".ToCharArray());
                if (abilities.Length == 3)
                {
                    int index = Int32.Parse(abilities[0]);
                    Vector2 area = new Vector2();
                    area.x = Int32.Parse(abilities[1]) * -0.001f;
                    area.y = Int32.Parse(abilities[2]) * -0.001f;

                    Players[1].Units[i].UnitOrder.Abilities.Add(new Tuple<int, Vector2>(index, area));
                }
            }

            ClearBuffer();
            EndTurn();
        }
    }

    public void EndTurn()
    {
        TempManager.Instance.TurnInput.gameObject.SetActive(false);
        TempManager.Instance.TurnOutput.gameObject.SetActive(false);

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
        TempManager.Instance.TurnButton.gameObject.SetActive(true);

        State = TurnState.WAITING;
        TurnCount++;
        currentLocalPlayer = 0;

        foreach (Player player in Players)
            foreach (Unit unit in player.Units)
                unit.UnitOrder.Abilities.Clear();
    }
}
