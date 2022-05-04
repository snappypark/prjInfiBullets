using UnityEngine;
namespace nj
{ 
[System.Serializable]
public struct scFloat
{
    [System.NonSerialized]
    static readonly int MAX_TSSDATA = 2;

    [System.NonSerialized]
    TssSdtFloat[] m_tssData;
    /*
#if UNITY_EDITOR
    [SerializeField]
    float data;
#endif*/

    bool IsSafe()
    {
        for (int i = 1; i < MAX_TSSDATA; i++)
        {
            float data1 = (float)m_tssData[i];
            float data2 = (float)m_tssData[i - 1];
            float dif = data1 - data2;
            if ( Mathf.Abs(dif) > 0.1f)
            {
                // 데이타가 변조됨
                sc_static.ProblemDetected("[SecureFloat] dif=" + dif + ", data1=" + data1 + ", data2=" + data2);
                return false;
            }
        }

        return true;
    }

    private float GetValue()
    {
        if (m_tssData != null)
        {
            if (IsSafe())
                return (float)m_tssData[0];
        }
        return 0;
    }
    private void SetValue(float v)
    {
        if( m_tssData == null )
            m_tssData = new TssSdtFloat[MAX_TSSDATA];

        for (int i = 0; i < MAX_TSSDATA; i++)
            m_tssData[i] = v;

        // 인스펙터에서 확인할수 있도록 값을 넣어준다.
        /*
#if UNITY_EDITOR
        data = v;
#endif*/

    }

    public static implicit operator float(scFloat v)
    {
        return v.GetValue();
    }

    public static implicit operator scFloat(float v)
    {
        scFloat obj = new scFloat();
        obj.SetValue(v);
        return obj;
    }

    public override string ToString()
    {
        return string.Format("{0}", GetValue());
    }
}
}