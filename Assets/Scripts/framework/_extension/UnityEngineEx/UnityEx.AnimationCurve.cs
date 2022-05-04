using System.Collections;
using UnityEngine;
using System.Reflection;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngineEx
{
    [System.Serializable]
    public class AniCurveEx
    {
        [SerializeField] protected AnimationCurve _curve;
        [SerializeField] protected float _maxTime = 1.0f;
        protected float _normalizeMaxTime = 1.0f;
        protected float _curTime = 0;

        public bool IsEnd{ get { return _curTime > _maxTime; }}

        public void ResetTime(float maxtime)
        {
            _maxTime = maxtime;
            switch (_curve.postWrapMode)
            {
                case WrapMode.PingPong:
                    _normalizeMaxTime = 2 / maxtime;
                    break;
                default:
                    _normalizeMaxTime = 1 / maxtime;
                    break;
            }
            _curTime = 0;
        }

        public void ResetTime()
        {
            ResetTime(_maxTime);
        }

        public void EndTime()
        {
            _curTime = _maxTime + 1.0f;
        }

        public bool UpdateUntilTime()
        {
            _curTime += Time.smoothDeltaTime;
            return _curTime < _maxTime;
        }

        public float Evaluate()
        {
            return _curve.Evaluate(_curTime * _normalizeMaxTime);
        }
    }
    
    [System.Serializable]
    public class AniCurveEx_TranScale : AniCurveEx
    {
        [SerializeField] Transform _trans;

        public void ResetTime_and_Color(float time, float scale)
        {
            ResetTime(time);
            _trans.localScale = new Vector3(scale, scale);
        }

        public void SetScale (float scale)
        {
            _trans.localScale = new Vector3(scale, scale);
        }

        public void Update(float initScale = 0.7f)
        {
            if (UpdateUntilTime())
                SetScale(initScale + Evaluate() * 0.19f);
            else
                ResetTime();
        }

        public IEnumerator UpdatePingpong_(float initScale)
        {
            ResetTime();
            SetScale(initScale);
            while (UpdateUntilTime())
            {
                SetScale(initScale + Evaluate() * 0.59f);
                yield return null;
            }
            SetScale(initScale);
            yield return null;
        }
    }


    [System.Serializable]
    public class AniCurveEx_RectTran : AniCurveEx
    {
        [SerializeField] RectTransform[] _trans;

        public IEnumerator Move_(Vector2[] from, Vector2[] to)
        {
            ResetTime();
            for (int i = 0; i < _trans.Length; ++i)
                _trans[i].anchoredPosition = from[i];

            while (UpdateUntilTime())
            {
                float value = Mathf.Clamp01(Evaluate());
                for (int i = 0; i < _trans.Length; ++i)
                    _trans[i].anchoredPosition = from[i] + (to[i] - from[i]) * value;
                yield return null;
            }
            for (int i = 0; i < _trans.Length; ++i)
                _trans[i].anchoredPosition = to[i];
            yield return null;
        }
        
    }


    [System.Serializable]
    public class AniCurveEx_GameObj_UIGraphics
    {
        [SerializeField] public GameObject rootGameObj;
        [SerializeField] public AniCurveEx_UIGraphics contexts;
    }

    [System.Serializable]
    public class AniCurveEx_UIGraphics : AniCurveEx
    {
        [SerializeField] UnityEngine.UI.Graphic[] _graphics;

        public void SetAlpha(float alpha)
        {
            int numGraphics = _graphics.Length;
            for (int i = 0; i < _graphics.Length; ++i)
                _graphics[i].color = _graphics[i].color.Alpha(alpha);
        }

        public IEnumerator FadeIn_()
        {
            ResetTime();
            int numGraphics = _graphics.Length;
            for (int i = 0; i < _graphics.Length; ++i)
                _graphics[i].color = _graphics[i].color.Alpha(0.0f);

            while (UpdateUntilTime())
            {
                float value = Mathf.Clamp01(Evaluate());
                for (int i = 0; i < numGraphics; ++i)
                    _graphics[i].color = _graphics[i].color.Alpha(value);
                yield return null;
            }
            for (int i = 0; i < numGraphics; ++i)
                _graphics[i].color = _graphics[i].color.Alpha(1.0f);
            yield return null;
        }

        public IEnumerator FadeOut_()
        {
            ResetTime();
            int numGraphics = _graphics.Length;
            for (int i = 0; i < numGraphics; ++i)
                _graphics[i].color = _graphics[i].color.Alpha(1.0f);

            while (UpdateUntilTime())
            {
                float value = Mathf.Clamp01(Evaluate());
                for (int i = 0; i < numGraphics; ++i)
                    _graphics[i].color = _graphics[i].color.Alpha(1-value);
                yield return null;
            }
            for (int i = 0; i < numGraphics; ++i)
                _graphics[i].color = _graphics[i].color.Alpha(0.0f);
            yield return null;
        }
    }

}
