using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public static AnimationManager Instance;

    List<AbilityTickable> _abilitiesToTick = new List<AbilityTickable>();

    void Awake()
    {
        if (Instance)
            Destroy(this);
        else
            Instance = this;
    }

    public void PlayAnimation()
    {
        StartCoroutine(MovementAnimation());
    }

    IEnumerator MovementAnimation()
    {
        int stepsToFinish = Mathf.CeilToInt(SettingManager.Instance.CurrentGameSetting.MovementTimeWindow / SettingManager.Instance.AnimationTimestep);
        int currentStep = 0;
        float timeSinceLastFrame = 0;

        foreach (Player player in TurnManager.Instance.Players)
        {
            foreach (Unit unit in player.Units)
            {
                if (!unit.IsDead && unit.HasOrder)
                {
                    AbilityTickable ability = unit.UnitAbilities[unit.UnitOrder.Abilities[0].Item1].AbilityLogic(unit.UnitOrder.Abilities[0].Item2);
                    if (ability)
                        _abilitiesToTick.Add(ability);
                }
            }
        }

        while (currentStep < stepsToFinish)
        {
            timeSinceLastFrame += Time.deltaTime;

            if (timeSinceLastFrame >= SettingManager.Instance.AnimationTimestep)
            {
                timeSinceLastFrame -= SettingManager.Instance.AnimationTimestep;
                currentStep++;

                for (int i = 0; i < _abilitiesToTick.Count; i++)
                {
                    if (_abilitiesToTick[i].Owner.IsDead || !_abilitiesToTick[i].TickMe(SettingManager.Instance.AnimationTimestep, currentStep == stepsToFinish))
                    {
                        Destroy(_abilitiesToTick[i].gameObject);
                        _abilitiesToTick.RemoveAt(i);
                        i--;
                        continue;
                    }
                }
            }

            yield return null;
        }

        TurnManager.Instance.NewTurn();
    }
}
