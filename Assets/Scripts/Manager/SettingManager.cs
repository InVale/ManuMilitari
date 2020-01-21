using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingManager : MonoBehaviour
{
    public static SettingManager Instance;

    [Header("General")]
    public GameSetting CurrentGameSetting;
    public int FixedFramePerSecondForAnimation = 60;

    [Header("Layers")]
    public LayerMask SelectionLayer;
    public LayerMask GroundLayer;
    public LayerMask ObstaclesLayer;
    public LayerMask MovementObstaclesLayer;
    public LayerMask VisionObstaclesLayer;

    [Header("Move Area Visualization")]
    public float MoveAreaResolution;
    public int MoveAreaEdgeIteration;
    public float MoveAreaEdgeDistanceThreshold;
    public float MoveAreaCutawayDistance;

    public float AnimationTimestep => 1.0f / FixedFramePerSecondForAnimation;

    void Awake()
    {
        if (Instance)
            Destroy(this);
        else
            Instance = this;
    }
}
