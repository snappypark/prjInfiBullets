using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class heroEditEntity : absHeroEntity
{
    [SerializeField] float _spd = 10;
    
    void OnEnable()
    {
        hero.ResetStat(-1);
        hero.ResetPos();
        
        hero.culling.Refresh();
    }

    void FixedUpdate()
    {
        hero.UpdateTran();
        hero.culling.OnUpdate(hero.pt.z);
        if(Input.GetKey(KeyCode.W))
            hero.MoveZ(_spd);
        if(Input.GetKey(KeyCode.S))
            hero.MoveZ(-_spd);  
        if(Input.GetKey(KeyCode.A))
            hero.MoveLeft(_spd);
        if(Input.GetKey(KeyCode.D))
            hero.MoveRight(_spd);
    }

}
