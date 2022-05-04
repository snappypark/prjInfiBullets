using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineEx;

public partial class hero
{
    public static void fxPlay_lvDown()
    {
        _inst._fxDown.SetBeginColor(stat.color);
        _inst._fxDown.Play();
    }

    public static void LvDown()
    {
        if(stat.lv == 0)
        {
            ofts.particles.Create(effParticles.hitHero, _inst._tr.localPosition);
            fxPlay_lvDown();
            hero.Init(hero.Type.End);
            return;
        }
        else if(stat.lv > 0)
        {
            stat.lv = 0;
            ofts.particles.Create(effParticles.hitHero, _inst._tr.localPosition);
            _inst.refreshByLv();
            fxPlay_lvDown();
        }
    }

    public static void LvUp()
    {
        ++stat.lv;
        _inst.refreshByLv();
        _inst._fxUp.SetBeginColor(stat.color);
        _inst._fxUp.Play();
    }

    public static stat_ stat = new stat_();
    public class stat_
    {
        public const float shootY = 0.47f;
        public int lv=0;
        public delay bulletDly = new delay(100.1f);
        public float bulletSpd = 10.0f;
        public Color color = Color.white;
        public float zSpeed;

        public void Set(float dly, float spd, Color color_)
        {
            bulletDly.Reset(dly); bulletSpd = spd; color = color_;
        }
        
        public static readonly Color lv1w = new Color(1,1,1, 0.9f);
        public static readonly Color lv2b = new Color(0.7490196f, 0.9019608f, 1, 0.9f);
        public static readonly Color lv3g = new Color(0.8352942f, 0.9960785f, 0.9019608f, 0.9f);
        public static readonly Color lv4o = new Color(0.9921569f, 0.8980393f, 0.7019608f, 0.9f);
        public static readonly Color lv5r = new Color(0.9921569f, 0.7764707f, 0.8941177f, 0.9f);
        public static readonly Color lv6p = new Color(0.7764707f, 0.7098039f, 0.9960785f, 0.9f);
        public static readonly Color lv7b = new Color(0.4901961f, 0.5568628f, 0.937255f, 0.9f);
        public static readonly Color lv8g = new Color(0.4627451f, 0.7568628f, 0.627451f, 0.9f);
        public static readonly Color lv9y = new Color(0.9882354f, 0.7019608f, 0.227451f, 0.9f);
        public static readonly Color lv10 = new Color(0.9843138f, 0.4627451f, 0.4627451f, 0.9f);

    }

    public static void ResetStat(int stageIdx)
    { 
        stat.lv = 0;
        stat.zSpeed = getZSpeedOfStage(stageIdx);
        _inst.refreshByLv();
    }
    
    void refreshByLv()
    {
        switch(stat.lv)
        {
            case 0: stat.Set(0.22f, 16.2f, stat_.lv1w); break;
            case 1: stat.Set(0.21f, 16.4f, stat_.lv2b); break;
            case 2: stat.Set(0.20f, 16.6f, stat_.lv3g); break;
            case 3: stat.Set(0.19f, 16.8f, stat_.lv4o); break;
            case 4: stat.Set(0.18f, 16.0f, stat_.lv5r); break;

            case 5: stat.Set(0.17f, 17.2f, stat_.lv6p); break;
            case 6: stat.Set(0.16f, 17.4f, stat_.lv7b); break;
            case 7: stat.Set(0.15f, 17.6f, stat_.lv8g); break;
            case 8: stat.Set(0.14f, 17.8f, stat_.lv9y); break;
            case 9: stat.Set(0.13f, 18.0f, stat_.lv10); break;
        }
        _inst._fxFire.SetBeginColor(new Color(stat.color.r, stat.color.g, stat.color.b, 0.5f));
        _spriter.SetColor(stat.color);
    }

    
    public static void Shoot()
    {
        if (stat.bulletDly.InTime())
            return;
        stat.bulletDly.Reset();

        int dirIdx = _inst._spriter.GetDirIdx();
        int aniIdx = _inst._spriter.GetAniIdx();

        f2 fxGaps = loads.fxGaps[dirIdx, aniIdx];
        f2 gapFire = loads.aimGaps[dirIdx, aniIdx];
        f2 dirFire = loads.aimDirs[dirIdx, aniIdx];
        
        Vector3 posFire = new Vector3(_inst._tr.localPosition.x + gapFire.x, stat_.shootY,
                                        _inst._tr.localPosition.z + gapFire.z);
        objs.bullets.Fire(posFire, dirFire.x, dirFire.z, stat.bulletSpd, stat.color);
        
        _inst._fxFire.transform.localPosition = new Vector3(_inst._tr.localPosition.x + fxGaps.x, stat_.shootY,
                                        _inst._tr.localPosition.z + fxGaps.z);
        FxFire();
    }



    //♩
    
    public static float getZSpeedOfStage(int stageIdx)
    {
        switch(stageIdx)
        {
            default: return 0.8f;
            case -1: return 3.4f;
        }
    }
}
