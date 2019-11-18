using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempManager : MonoBehaviour
{
    public static TempManager Instance;

    [SerializeField] private GameObject[] _playerTeam;
    [SerializeField] private GameObject[] _enemyTeam;

    [Space]
    [SerializeField] private Transform[] _playerSpawns;
    [SerializeField] private Transform[] _enemySpawns;

    [Header("Assets")]
    public Material MovementAreaMaterial;

    void Awake()
    {
        if (Instance)
            Destroy(this);
        else
            Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Player player = new Player();
        for (int i = 0; i < _playerTeam.Length; i++)
        {
            Unit unit = GameObject.Instantiate(_playerTeam[i], _playerSpawns[i].position, _playerSpawns[i].rotation).GetComponent<Unit>();
            unit.Init(player, i, _playerSpawns[i]);
            player.Units.Add(unit);
        }

        TurnManager.Instance.LocalPlayers.Add(player);
        TurnManager.Instance.Players.Add(player);

        player = new Player();
        for (int i = 0; i < _enemyTeam.Length; i++)
        {
            Unit unit = GameObject.Instantiate(_enemyTeam[i], _enemySpawns[i].position, _enemySpawns[i].rotation).GetComponent<Unit>();
            unit.Init(player, i, _enemySpawns[i]);
            player.Units.Add(unit);
        }
        TurnManager.Instance.DebugPlayers.Add(player);
        TurnManager.Instance.Players.Add(player);

        LogManager.Instance.PlayerNumber = 2;
    }
}
