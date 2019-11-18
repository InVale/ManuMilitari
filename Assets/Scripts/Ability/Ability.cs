using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbilityFiringTiming
{
    AFTER_MOVEMENT,
    BEFORE_MOVEMENT
}

public abstract class Ability : ScriptableObject
{
    [Header("General")]
    public Sprite AbilityIcon;

    [NonSerialized] public Unit Owner;
    [NonSerialized] public int ID;

    public abstract void InitAiming();
    public abstract void UpdateAiming(Vector3 target);
    public abstract void EndAiming();

    //Return false if the input isn't correct
    public abstract bool VisualizeAbility(Vector3 target);
    public abstract void EndVisualization();

    public abstract AbilityTickable AbilityLogic(Vector2 target);

    public abstract void CleanForDestruction();
}