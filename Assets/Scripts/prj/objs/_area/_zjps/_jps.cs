using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _jps : qObj
{
    [SerializeField] public TextMesh[] lbs;
    
    public void OnActive(cell c)
    {
        for(int i=0; i<8; ++i)
            lbs[i].text = c.jpDists[i].ToString();
    }
}
