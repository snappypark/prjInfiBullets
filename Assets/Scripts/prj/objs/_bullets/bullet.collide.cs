using UnityEngine;

public partial class bullet
{
    const float colliGap = 0.75f;
    bool collideByCubeSide(cell c)
    {
        float diffX = transform.localPosition.x - c.ct.x;
        float diffZ = transform.localPosition.z - c.ct.z;

        if(-0.51f < diffZ && diffZ < 0.51f)
            if(transform.localPosition.x > c.ct.x && diffX < colliGap && transform.forward.x < 0 ) {
                transform.forward = new Vector3(-transform.forward.x, 0, transform.forward.z);
                return true; }
            else if(transform.localPosition.x < c.ct.x && diffX > -colliGap && transform.forward.x > 0 ) {
                transform.forward = new Vector3(-transform.forward.x, 0, transform.forward.z);
                return true; }

        if(-0.51f < diffX && diffX < 0.51f)
            if(transform.localPosition.z > c.ct.z && diffZ < colliGap && transform.forward.z < 0 ) {
                transform.forward = new Vector3(transform.forward.x, 0, -transform.forward.z);
                return true; }
            else if(transform.localPosition.z < c.ct.z && diffZ > -colliGap && transform.forward.z > 0 ) {
                transform.forward = new Vector3(transform.forward.x, 0, -transform.forward.z);
                return true; }
        return false;
    }

    void collideByCubePoint(f2 ct)
    {
        float diffX = transform.localPosition.x - ct.x;
        float diffZ = transform.localPosition.z - ct.z;
        if(diffX*diffX + diffZ*diffZ < sqr.f0_03)
        {
            transform.forward = new Vector3(diffX, 0, diffZ).normalized;
            //ofts.particles.Create(effParticles.hitWall, transform.localPosition);
        }
    }

    bool collideByBaller(f2 ct)
    {
        float diffX = transform.localPosition.x - ct.x;
        float diffZ = transform.localPosition.z - ct.z;
        if(diffX*diffX + diffZ*diffZ < sqr.f0_64){
            transform.forward = new Vector3(diffX, 0, diffZ).normalized;return true; }
        return false;
    }

    void collideByBall(cell c)
    {
        bool isOnColli = false;
        float sumx= 0; float sumz= 0;
        for (int t = 0; t < c.cdxs.Count; ++t)
        {
            ball ball = objs.balls[c.cdxs.Dequeue()];

            float diffX = transform.localPosition.x - ball.transform.localPosition.x;
            float diffZ = transform.localPosition.z - ball.transform.localPosition.z;
            if(diffX*diffX + diffZ*diffZ < sqr.f0_64)
            {
                sumx += diffX;
                sumz += diffZ;
                isOnColli = true;
                ball.HitByBullet(c);
                continue;
            }
            c.cdxs.Enqueue(ball.cdx);
        }
        
        if (isOnColli)
        {
            //ofts.particles.Create(effParticles.hitWall, transform.localPosition);
            transform.forward = new Vector3(sumx,0, sumz).normalized;
        }
    }
}
