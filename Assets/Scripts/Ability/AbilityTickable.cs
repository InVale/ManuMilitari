using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityTickable : MonoBehaviour
{
    float _duration;

    public void Init (float duration)
    {
        _duration = duration;
    }

    //Return false if ticking should stop
    public virtual bool TickMe(float delta, bool finalTick)
    {
        _duration -= delta;
        return _duration > 0;
    }
}
