using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityTickable : MonoBehaviour
{
    public Unit Owner;
    protected float _duration;

    public void Init (Unit owner, float duration)
    {
        _duration = duration;
        Owner = owner;
    }

    //Return false if ticking should stop
    public virtual bool TickMe(float delta, bool finalTick)
    {
        _duration -= delta;
        return _duration > 0;
    }
}
