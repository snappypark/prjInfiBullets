using UnityEngine;

public class heroBeginEnity : absHeroEntity
{
    delay _delay = new delay(0.33f);
    void OnEnable()
    {
        hero.ShowSprite(true);
        hero.ResetPos();
        hero.culling.Refresh();
        hero.UpdateTran();
        hero.culling.OnUpdate(hero.pt.z);
        _delay.Reset();
    }

    void FixedUpdate()
    {
        uis.cover.Img.SetAlpha(_delay.Ratio10()*heroEndEntity.alpha);
        if(_delay.afterOnceTime())
        {
            uis.cover.Img.SetAlpha(0);
            enabled = false;
            hero.Init(hero.Type.Play);
        }
    }
}
