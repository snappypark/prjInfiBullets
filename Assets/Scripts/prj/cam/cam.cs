using System.Collections;
using UnityEngine;
using UnityEngineEx;

public struct CamOrthInfo
{
    public Vector3 Pos;
    public float OrthSize;
    public CamOrthInfo(Vector3 pos, float orthSize)
    {
        Pos = pos;
        OrthSize = orthSize;
    }
}

public class cam : MonoBehaviour
{
    static cam _inst;
    [SerializeField] AniCurveEx _moveAni;
    [SerializeField] public Camera Main;
    [SerializeField] public Vector2 gapYZ;
    [SerializeField] public float angle = 45;

    void Awake()
    {
        _inst = this;
        scr.Awake();
    }

    public static void SetTranInfo(Vector2 gapYZ_, float angle_)
    {
        _inst.gapYZ = gapYZ_;
        _inst.angle = angle_;
    }

    public static void OnUpdateByHero(Transform heroTran)
    {
        //_inst.transform.localPosition = new Vector3(6.5f, _inst.gapYZ.x, heroTran.localPosition.z + _inst.gapYZ.y);


        float gap = (heroTran.localPosition.x - 6.5f)*0.5f;
        _inst.transform.localPosition = new Vector3( 
            6.5f + gap, _inst.gapYZ.x, heroTran.localPosition.z + _inst.gapYZ.y);
        _inst.transform.localEulerAngles = new Vector3(_inst.angle, -gap, gap*0.4f);    
    }

    public void SetOrthInfo(CamOrthInfo info)
    {
        Main.transform.localPosition = info.Pos;
        Main.orthographicSize = info.OrthSize;
    }

    public IEnumerator SetOrthInfo_(float time, CamOrthInfo info)
    {
        _moveAni.ResetTime(time);
        while (_moveAni.UpdateUntilTime())
        {
            float t = _moveAni.Evaluate();
            Main.transform.localPosition
                = Vector3.Lerp(Main.transform.localPosition, info.Pos, t);
            Main.orthographicSize
                = Mathf.Lerp(Main.orthographicSize, info.OrthSize, t);
            yield return null;
        }
        SetOrthInfo(info);
        yield return null;
    }
    
    static class scr
    {
        public static float AspectMin = 1.5f;  //  for height
        public static float Aspect3_2 = 0.6666666667f;  //  for height
        public static float AspectW_H = 0.0f;  // for width
        public static float AspectH_W = 0.0f;  //  for height

        public static int Width = 0;
        public static int Height = 0;
        public static float OverWidth = 0;
        public static float OverHeight = 0;
        public static float HalfWidth = 0;
        public static float HalfHeight = 0;
        public static float HalfOverWidth = 0;
        public static float HalfOverHeight = 0;

        
            const int scrCommonHeight = 860;//920;//860// 960;//980;//1080;// 1280
#if UNITY_ANDROID || UNITY_IOS
            static int scrHeight = scrCommonHeight;//890// 960;//980;//1080;// 1280//1600
#else
            static int scrHeight = scrCommonHeight;
#endif
        public static void Awake()
        {
            Width = Screen.width;
            Height = Screen.height;
            OverWidth = 1 / Width;
            OverHeight = 1 / Height;
            HalfWidth = Screen.width * 0.5f;
            HalfHeight = Screen.height * 0.5f;
            HalfOverWidth = 1 / HalfWidth;
            HalfOverHeight = 1 / HalfHeight;

            AspectW_H = (float)Screen.width / (float)Screen.height;
            AspectH_W = (float)Screen.height / (float)Screen.width;

            
#if UNITY_EDITOR
                Height = Screen.height;
#elif UNITY_ANDROID || UNITY_IOS
                Height = Mathf.Min(Screen.currentResolution.height, scrHeight);
#else
                Height = Screen.height ;
#endif
                //  Width = Screen.width;
                //  Height = Screen.height;
                Width = (int)(AspectW_H * Height);
                //Debug.Log("1Screen.Height " + Height);
                //   Debug.Log("Screen.width " + Width);
                Screen.SetResolution(Width, Height, true);

        }
    }
}
