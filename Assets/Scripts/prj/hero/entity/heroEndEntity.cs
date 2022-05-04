using UnityEngine;

public class heroEndEntity : absHeroEntity
{
    delay _delay = new delay(0.27f);
    void OnEnable()
    {
        resetDirection();
        effConfettis.Shuffle();
        hero.ShowSprite(false);
        _delay.Reset();
    }

    public const float alpha = 0.89f;
    void FixedUpdate()
    {
        uis.cover.Img.SetAlpha(_delay.Ratio01()*alpha);
        if(_delay.afterOnceTime())
        {
            uis.cover.Img.SetAlpha(alpha);
            flows.Change<flowPlay>();
            enabled = false;
        }
    }
}
