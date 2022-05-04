using UnityEngine;

public class heroGoalEntity : absHeroEntity
{
    delay _delay = new delay(2.8f);
    delay _enddelay = new delay(0.27f, false);
    delay _delayEff = new delay(0.4f);
    void OnEnable()
    {
        effConfettis.Shuffle();
        area.end.Enter();
        resetDirection();
        _delay.Reset();
        _delayEff.Reset();
    }

    void Update()
    {
        if(_delayEff.IsEnd())
        { 
            effConfettis.Fire(endfield.z);
            _delayEff.Reset();
        }

        if(_delay.afterOnceTime())
        {
            hero.ShowSprite(false);
            hero.fxPlay_lvDown();
            _enddelay.Reset();
        }
        else if(_delay.InTime())
        {
            hero.UpdateSpriteIdx();
            hero.UpdateTran();
            hero.MoveZ(3.0f);
            hero.MoveCenter(1.5f);
        }

        if(_enddelay.afterOnceTime())
        {
            dStage.AddStage();
            dCore.Save_PlayInfo();
            flows.Change<flowPlay>();
            enabled = false;
        }
        else if(_enddelay.InTime())
            uis.cover.Img.SetAlpha(_enddelay.Ratio01()*heroEndEntity.alpha);
    }
}
