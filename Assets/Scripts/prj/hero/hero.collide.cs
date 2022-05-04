using UnityEngine;

public partial class hero
{
    const float _heroRadius = 0.29f;
    const float _heroSqrRadius = _heroRadius*_heroRadius;
    public static delay dlyColli = new delay(1.7f);

    public static bool CollideByCube(cell c, cell c11) {
        return CollideByCubeSide(c) || CollideByRadius(c11.pt, _heroSqrRadius); }

    public static bool CollideByTrapSmall(cell c, cell c11) {
        return CollideByCubeSide(c) || CollideByRadius(c11.pt, sqr.f0_25); }

    public static bool CollideByRadius(i2 pos, float sqrRadius)
    {
        float diffX = _inst._tr.localPosition.x - pos.x;
        float diffZ = _inst._tr.localPosition.z - pos.z;
        return diffX*diffX + diffZ*diffZ < sqrRadius;
    }
    public static bool CollideByRadius(f2 pos, float sqrRadius)
    {
        float diffX = _inst._tr.localPosition.x - pos.x;
        float diffZ = _inst._tr.localPosition.z - pos.z;
        return diffX*diffX + diffZ*diffZ < sqrRadius;
    }

    const float colliGap = 0.5f + _heroRadius;
    public static bool CollideByCubeSide(cell c)
    {
        float diffX = _inst._tr.localPosition.x - c.ct.x;
        float diffZ = _inst._tr.localPosition.z - c.ct.z;

        if(-0.51f < diffZ && diffZ < 0.51f)
            if(_inst._tr.localPosition.x > c.ct.x && diffX < colliGap )
                return true;
            else if(_inst._tr.localPosition.x < c.ct.x && diffX > -colliGap )
                return true;

        if(-0.51f < diffX && diffX < 0.51f)
            if(_inst._tr.localPosition.z > c.ct.z && diffZ < colliGap )
                return true;
            else if(_inst._tr.localPosition.z < c.ct.z && diffZ > -colliGap )
                return true;
        return false;
    }

    public static void CollideByBall(cell c)
    {
        for (int t = 0; t < c.cdxs.Count; ++t)
        {
            ball ball = objs.balls[c.cdxs.Dequeue()];

            float diffX = _inst._tr.localPosition.x - ball.transform.localPosition.x;
            float diffZ = _inst._tr.localPosition.z - ball.transform.localPosition.z;
            if(diffX*diffX + diffZ*diffZ < sqr.f0_74)
            {
                objs.balls.Inactive(ball.cdx);
                hero.LvDown();
                continue;
            }
            c.cdxs.Enqueue(ball.cdx);
        }
    }
}
