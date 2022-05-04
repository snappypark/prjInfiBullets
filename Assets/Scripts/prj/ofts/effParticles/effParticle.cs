using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class effParticle : qObj
{
    [SerializeField] public ParticleSystem[] PS;
    
    public override void OnUnuse()
    {
        for (int i = 0; i < PS.Length; ++i)
        {
            PS[i].Stop();
            PS[i].Clear();
        }
    }
    
}
