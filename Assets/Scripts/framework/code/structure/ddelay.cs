using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ddelay
{
    delay _dealy;
    public ddelay(float duration_, bool onceAfterTime_ = true)
    {
        _dealy = new delay(duration_);
    }
    
}
